using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Alteruna;
using UnityEditor;
using System;

public class MusicPlayer : AttributesSync
{
    public AudioSource musicPlayer;
    public List<AudioClip> songs;

    List<int> queue;
    bool shuffle;
    bool playingMusic;

    int _id = -1;

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

        queue = new List<int>();

        for (int i = 0; i < songs.Count; i++)
        {
            queue.Add(i);
        }

        ShuffleQueue();
    }


    private void Update()
    {
        if (Multiplayer.Instance.InRoom)
        {
            if (GameManager.Instance.user.Index == Multiplayer.Instance.LowestUserIndex)
            {
                if (playingMusic && !musicPlayer.loop && !musicPlayer.isPlaying)
                {
                    InvokeRemoteMethod(nameof(ChangeSong), Multiplayer.Instance.LowestUserIndex, (_id + 1 < songs.Count) ? _id + 1 : 0);
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleIsPlaying(!musicPlayer.isPlaying);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Playing new song...");
            InvokeRemoteMethod(nameof(ChangeSong), Multiplayer.Instance.LowestUserIndex, (_id + 1 < songs.Count) ? _id + 1 : 0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Toggled shuffle...");
            ChangeShuffle(!shuffle);
        }
    }

    void ShuffleQueue()
    {
        queue = queue.OrderBy(a => Guid.NewGuid()).ToList();
    }

    public void ChangeShuffle(bool value)
    {
        InvokeRemoteMethod(nameof(SynchEnableShuffle), UserId.AllInclusive, value);
    }

    [SynchronizableMethod]
    void SynchEnableShuffle(bool value)
    {
        shuffle = value;
        if (shuffle)
        {
            ShuffleQueue();
        }
        else
        {
            _id = queue[_id];
        }
    }

    public void ToggleIsPlaying(bool isPlaying)
    {
        InvokeRemoteMethod(nameof(SynchToggleIsPlaying), UserId.AllInclusive, isPlaying);
    }

    [SynchronizableMethod]
    void SynchToggleIsPlaying(bool isPlaying)
    {
        if (isPlaying)
            musicPlayer.UnPause();
        else
            musicPlayer.Pause();
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
        InvokeRemoteMethod(nameof(SyncPlayback), UserId.All, musicPlayer.time, System.DateTime.Now.Ticks, _id);
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
            _id = id;
        }

    }

    [SynchronizableMethod]
    public void ChangeSong(int ID)
    {
        playingMusic = true;
        Debug.Log("Changed new song to: " + ID);
        if (ID < 0)
        {
            playingMusic = false;
            musicPlayer.clip = null;
            musicPlayer.Stop();
            return;
        }

        musicPlayer.clip = songs[shuffle ? queue[ID] : ID];
        musicPlayer.Play();
        _id = shuffle ? queue[ID] : ID;
        SynchAll();

    }
}
