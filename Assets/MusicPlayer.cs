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
       
    }


    [SynchronizableMethod]
    public void ChangeSong(int ID)
    {
        if (ID == -1)
        {
            musicPlayer.clip = null;
            return;
        }

        musicPlayer.clip = songs[ID];
    }
}
