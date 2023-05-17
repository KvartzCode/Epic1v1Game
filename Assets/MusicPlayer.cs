using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class MusicPlayer : AttributesSync
{
    public AudioSource musicPlayer;
    public List<AudioClip> songs;

    int id;

    private void Awake()
    {
        musicPlayer = GetComponent<AudioSource>();

    }

    private void Start()
    {
        SynchAll();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Playing new song...");
            InvokeRemoteMethod(nameof(ChangeSong), UserId.AllInclusive, Random.Range(0, songs.Count));
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
        InvokeRemoteMethod(nameof(SyncPlayback), UserId.AllInclusive, musicPlayer.time, id);
    }

    [SynchronizableMethod]
    void SyncPlayback(float pos, int id)
    {
        musicPlayer.clip = songs[id];
        musicPlayer.Play();
        musicPlayer.time = pos;

    }

    [SynchronizableMethod]
    public void ChangeSong(int ID)
    {
        Debug.Log("Changed new song to: " + ID);
        if (ID == -1)
        {
            musicPlayer.clip = null;
            musicPlayer.Play();
            return;
        }

        musicPlayer.clip = songs[ID];
        musicPlayer.Play();
        id = ID;
    }
}
