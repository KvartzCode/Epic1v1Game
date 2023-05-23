using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class AudioManager : AttributesSync
{
    [SerializeField]
    private AudioClip[] soundEffects;
    [SerializeField]
    private GameObject[] players; // Array or list of players

    public void PlayGlobal3DSoundEffect(int soundEffectID, float volume, float radius, Vector3 position)
    {
        InvokeRemoteMethod(nameof(PlayGlobal3DSoundEffectMULT), UserId.All, soundEffectID, volume, radius, position);
        PlayLocal3DSoundEffect(soundEffectID, volume, radius, position);
    }
    [SynchronizableMethod]
    private void PlayGlobal3DSoundEffectMULT(int soundEffectID, float volume, float radius, Vector3 position)
    {
        AudioSource source = CreateAudioSource(soundEffectID, volume, radius, position);
        source.spatialBlend = 1f; // 3D sound
        source.Play();
        StartCoroutine(DestroyAfterPlaying(source.gameObject));
    }


    public void PlayGlobal2DSoundEffect(int soundEffectID, float volume, float radius)
    {
        InvokeRemoteMethod(nameof(PlayGlobal2DSoundEffectMULT), UserId.All, soundEffectID, volume, radius);
        PlayLocal2DSoundEffect(soundEffectID, volume, radius);
    }
    [SynchronizableMethod]
    public void PlayGlobal2DSoundEffectMULT(int soundEffectID, float volume, float radius)
    {
        AudioSource source = CreateAudioSource(soundEffectID, volume, radius, Vector3.zero);
        source.spatialBlend = 0f; // 2D sound
        source.Play();
        StartCoroutine(DestroyAfterPlaying(source.gameObject));
    }


    public void PlayGlobal3DSoundEffectAtPlayer(int soundEffectID, float volume, float radius, int playerID)
    {
        InvokeRemoteMethod(nameof(PlayGlobal3DSoundEffectAtPlayerMULT), UserId.All, soundEffectID, volume, radius, playerID);
        PlayLocal2DSoundEffect(soundEffectID, volume, radius);
    }
    [SynchronizableMethod]
    public void PlayGlobal3DSoundEffectAtPlayerMULT(int soundEffectID, float volume, float radius, int playerID)
    {
        GameObject player = players[playerID];
        AudioSource source = CreateAudioSource(soundEffectID, volume, radius, player.transform.position);
        source.transform.parent = player.transform; // Attach AudioSource to the player
        source.spatialBlend = 1f; // 3D sound
        source.Play();
        StartCoroutine(DestroyAfterPlaying(source.gameObject));
    }


    public void Play3DSoundEffectForSpecificPlayer(int soundEffectID, float volume, float radius, Vector3 position, int playerID)
    {
        if (playerID == Multiplayer.Instance.Me.Index)
        {
            PlayLocal3DSoundEffect(soundEffectID, volume, radius, position);
            return;
        }

        InvokeRemoteMethod(nameof(Play3DSoundEffectForSpecificPlayerMULT), UserId.All, soundEffectID, volume, radius, position, playerID);
 
    }
    [SynchronizableMethod]
    public void Play3DSoundEffectForSpecificPlayerMULT(int soundEffectID, float volume, float radius, Vector3 position, int playerID)
    {
        if (playerID == Multiplayer.Instance.Me.Index)
        {
            PlayLocal3DSoundEffect(soundEffectID, volume, radius, position);
        }
    }

    public void Play2DSoundEffectForSpecificPlayer(int soundEffectID, float volume, float radius, int playerID)
    {
        if (playerID == Multiplayer.Instance.Me.Index)
        {
            PlayLocal2DSoundEffect(soundEffectID, volume, radius);
            return;
        }

        InvokeRemoteMethod(nameof(Play2DSoundEffectForSpecificPlayerMULT), UserId.All, soundEffectID, volume, radius, playerID);

    }
    [SynchronizableMethod]
    public void Play2DSoundEffectForSpecificPlayerMULT(int soundEffectID, float volume, float radius, int playerID)
    {
        if (playerID == Multiplayer.Instance.Me.Index)
        {
            PlayLocal2DSoundEffect(soundEffectID, volume, radius);
        }
    }

    public void PlayLocal2DSoundEffect(int soundEffectID, float volume, float radius)
    {
        AudioSource source = CreateAudioSource(soundEffectID, volume, radius, this.transform.position);
        source.spatialBlend = 0f; // 2D sound
        source.Play();
        StartCoroutine(DestroyAfterPlaying(source.gameObject));
    }

    public void PlayLocal3DSoundEffect(int soundEffectID, float volume, float radius,Vector3 position)
    {
        AudioSource source = CreateAudioSource(soundEffectID, volume, radius, position);
        source.spatialBlend = 1f; // 3D sound
        source.Play();
        StartCoroutine(DestroyAfterPlaying(source.gameObject));
    }

    private AudioSource CreateAudioSource(int soundEffectID, float volume, float radius, Vector3 position)
    {
        GameObject audioObject = new GameObject("AudioSource_" + soundEffectID);
        audioObject.transform.position = position;

        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.clip = soundEffects[soundEffectID];
        source.volume = volume;
        source.spatialBlend = radius;

        // Configure the AudioSource for 3D sound.
        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 1f;
        source.maxDistance = radius;

        return source;
    }


    private IEnumerator DestroyAfterPlaying(GameObject obj)
    {
        yield return new WaitForSeconds(obj.GetComponent<AudioSource>().clip.length);
        Destroy(obj);
    }
}
