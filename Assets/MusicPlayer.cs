using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class MusicPlayer : AttributesSync
{
    public AudioSource musicPlayer;
    public List<AudioClip> songs;
    private void Awake()
    {
        musicPlayer = GetComponent<AudioSource>();

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Playing new song...");
            InvokeRemoteMethod(nameof(ChangeSong), UserId.AllInclusive, Random.Range(0, songs.Count));
        }
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
    }
}
