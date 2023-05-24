using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Alteruna;

public class TextToSpeech : AttributesSync
{
    public string message;
    private string pathToExe;
    private string wavFileName;
    private bool processFinished = false;
    private int TTSPlayerID;

    private void Start()
    {
        pathToExe = Path.Combine(Application.streamingAssetsPath, "dectalk/say.exe");
        Debug.Log($"pathToExe: {pathToExe}");
        Debug.Log($"pathToWav: {wavFileName}");
    }

    private void Update()
    {
        if (processFinished)
        {
            Debug.Log("Process finished. Loading audio...");
            processFinished = false;
            LoadAudioClip();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Y key pressed. Speaking...");
            GameManager.Instance.audioManager.PlayGlobalTTSAtPlayer(message, Multiplayer.Instance.Me.Index);
        }
    }

    [SynchronizableMethod]
    public void Speak(string text, int playerID)
    {
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

        TTSPlayerID = playerID;
        wavFileName = Path.Combine(Application.streamingAssetsPath, "dectalk/player" + playerID + ".wav");
        text = "[:phoneme on] " + text;
        Debug.Log($"Speaking: {text}");

        ProcessStartInfo startInfo = new ProcessStartInfo(pathToExe);
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.CreateNoWindow = true;  // This prevents the process from creating a window
        startInfo.Arguments = "-w \"Player" + playerID + ".wav\" " + $"\"{text}\"";
        Debug.Log("Sending info.. " + "-w \"Player" + playerID + ".wav\" " + $"\"{text}\"");
        startInfo.UseShellExecute = false;

        // Set the working directory to the dectalk folder
        startInfo.WorkingDirectory = Path.Combine(Application.streamingAssetsPath, "dectalk");

        Debug.Log("Starting process...");
        Process process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.Exited += new System.EventHandler(process_Exited);
        process.Start();
#endif
    }



    private void process_Exited(object sender, System.EventArgs e)
    {
        Debug.Log("Process exited.");
        processFinished = true;
    }

    private void LoadAudioClip()
    {
        StartCoroutine(LoadAudioClipRoutine());
    }

    private IEnumerator LoadAudioClipRoutine()
    {
        string url = $"file://{wavFileName}";
        Debug.Log($"Loading audio from: {url}");
        using (WWW www = new WWW(url))
        {
            yield return www;
            if (www.error == null)
            {
                Debug.Log("Audio clip loaded. Playing...");
                GameManager.Instance.audioManager.PlayGlobalTTSAtPlayerLOCAL(www.GetAudioClip(), TTSPlayerID);
            }
            else
            {
                Debug.LogError($"Failed to load audio clip: {www.error}");
            }
        }
    }
}
