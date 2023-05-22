using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class MusicPlayer : AttributesSync
{
    public AudioSource musicPlayer;
    public List<AudioClip> songs;

    int id = -1;

    private static MusicPlayer _instance;
    public static MusicPlayer Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Debug.LogError("More than one music player in scene");
        else
            _instance = this;

        musicPlayer = GetComponent<AudioSource>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Playing new song...");
            InvokeRemoteMethod(nameof(ChangeSong), Multiplayer.Instance.LowestUserIndex, Random.Range(0, songs.Count));
        }
    }



    public void SynchAll()
    {
        InvokeRemoteMethod(nameof(GetLowestSynch), Multiplayer.LowestUserIndex);
        if (!musicPlayer.isPlaying)
            musicPlayer.Play();
    }

    [SynchronizableMethod]
    void GetLowestSynch()
    {
        InvokeRemoteMethod(nameof(SyncPlayback), UserId.All, musicPlayer.time, System.DateTime.Now.Ticks, id);
    }

    [SynchronizableMethod]
    void SyncPlayback(float pos, long DatetimeTicks, int id)
    {
        if (id >= 0 && id < songs.Count)
        {

            #region OLD
            //Debug.LogWarning(datetimeMilliseconds);
            //float milicalc = (float)datetimeMilliseconds / 1000;
            //Debug.LogWarning(milicalc);
            //float then = datetimeSeconds + milicalc;
            //milicalc = (float)System.DateTime.Now.Millisecond / 1000;
            //float now = System.DateTime.Now.Second + milicalc;

            //Debug.LogError("___");
            //Debug.Log("now sec: " + System.DateTime.Now.Second + " |then mili" + milicalc);
            //milicalc = datetimeMilliseconds / 1000;
            //Debug.Log("then sec: " + datetimeSeconds + " |then mili" + milicalc);
            //Debug.Log("then: " + then + " | now: " + now);
            //if (then > now)
            //{
            //    Debug.Log("new now: " + now);
            //    now += 60;
            //}

            //Debug.Log("diff: " + (now - then) + " | pos: " + pos);
            //pos += now - then;
            //Debug.Log("Selected pos: " + pos);
            #endregion

            long elapsedTicks = System.DateTime.Now.Ticks - DatetimeTicks;

            System.TimeSpan timespan = new System.TimeSpan(elapsedTicks);

            float seconds = (float)timespan.TotalSeconds;

            pos += seconds;

            if (pos > songs[id].length)
                pos -= songs[id].length;


            musicPlayer.clip = songs[id];
            musicPlayer.Play();
            musicPlayer.time = pos;
        }

    }

    [SynchronizableMethod]
    public void ChangeSong(int ID)
    {
        Debug.Log("Changed new song to: " + ID);
        if (ID < 0)
        {
            musicPlayer.clip = null;
            musicPlayer.Stop();
            return;
        }


        musicPlayer.clip = songs[ID];
        musicPlayer.Play();
        id = ID;
        SynchAll();

    }
}
