using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Alteruna;

public class TextToSpeech : AttributesSync
{
    public AudioSource audioSource;
    public string message;
    private string pathToExe;
    private string wavFileName;
    private bool processFinished = false;

    private void Start()
    {
        pathToExe = Path.Combine(Application.streamingAssetsPath, "dectalk/debug.bat");
        wavFileName = Path.Combine(Application.streamingAssetsPath, "dectalk/player1.wav");
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
            InvokeRemoteMethod(nameof(Speak), UserId.AllInclusive, message);
        }
    }

    [SynchronizableMethod]
    public void Speak(string text)
    {
        text = "[:phoneme on] " + text;
        Debug.Log($"Speaking: {text}");

        ProcessStartInfo startInfo = new ProcessStartInfo(pathToExe);
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
        startInfo.Arguments = $"\"{text}\"";
        startInfo.UseShellExecute = true;

        Debug.Log("Starting process...");
        Process process = new Process();
        process.StartInfo = startInfo;
        process.EnableRaisingEvents = true;
        process.Exited += new System.EventHandler(process_Exited);
        process.Start();
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
                audioSource.clip = www.GetAudioClip();
                audioSource.Play();
            }
            else
            {
                Debug.LogError($"Failed to load audio clip: {www.error}");
            }
        }
    }
}
