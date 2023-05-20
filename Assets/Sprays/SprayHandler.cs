using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Networking;
using Alteruna;

public class SprayHandler : AttributesSync
{
    [SerializeField] private List<Material> sprayDecals;
    [SerializeField] private GameObject[] sprayObjects;
    [SerializeField] private Texture2D defaultSpray;
    [SerializeField] private string localImageURL = "";
    private static SprayHandler _instance;
    public static SprayHandler Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Debug.LogError("More than one spray handler in scene");
        else
            _instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //InvokeRemoteMethod(nameof(SynchedSetUserSpray), UserId.AllInclusive, Multiplayer.Instance.Me.Index, localImageURL);
        }
    }

    public void UpdatePosAll()
    {
        InvokeRemoteMethod(nameof(SynchedUpdatePosAll), UserId.AllInclusive);
    }

    [SynchronizableMethod]
    void SynchedUpdatePosAll()
    {
        int index = GameManager.Instance.user.Index;
        Spray(index, sprayObjects[index].transform.position, sprayObjects[index].transform.forward);
    }



    public void Spray(int userId, Vector3 pos, Vector3 reverseForward)
    {
        InvokeRemoteMethod(nameof(SynchedSpray), UserId.AllInclusive, userId, pos, reverseForward);
    }

    [SynchronizableMethod]
    void SynchedSpray(int userId, Vector3 pos, Vector3 reverseForward)
    {
        if (userId >= 0 && userId < sprayObjects.Length)
        {
            sprayObjects[userId].transform.position = pos;
            sprayObjects[userId].transform.forward = reverseForward;
        }
        else
            Debug.LogError("WAS SENT A USERID OUTSIDE OF RANGE OF SPRAYOBJECTS RANGE");
    }

    public void UpdateUserSpray()
    {
        InvokeRemoteMethod(nameof(SynchedSetUserSpray), UserId.AllInclusive, GameManager.Instance.user.Index, localImageURL);
    }

    public void GetAllSprays()
    {
        InvokeRemoteMethod(nameof(GiveSpray), UserId.All, GameManager.Instance.user.Index);
    }
    [SynchronizableMethod]
    void GiveSpray(int userId)
    {
        InvokeRemoteMethod(nameof(SynchedSetUserSpray), System.Convert.ToUInt16(userId), GameManager.Instance.user.Index, localImageURL);
    }

    public void SetSpray(string imageUrl)
    {
        localImageURL = imageUrl;

        if (Multiplayer.Instance.InRoom)
            UpdateUserSpray();
    }

    [SynchronizableMethod]
    void SynchedSetUserSpray(int userID, string url)
    {
        Debug.Log("Set user " + userID + "'s spray");
        StartCoroutine(LoadTextureFromURL(userID, url));
    }

    private IEnumerator LoadTextureFromURL(int userID, string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web request error: " + www.error);
            if (sprayDecals[userID].GetTexture("Base_Map") == null)
                sprayDecals[userID].SetTexture("Base_Map", defaultSpray);
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (myTexture != null)
            {
                Debug.Log("Successfully loaded texture from " + url + ", size: " + myTexture.width + "x" + myTexture.height);
                sprayDecals[userID].SetTexture("Base_Map", myTexture);
            }
            else
            {
                Debug.Log("Failed to load texture from " + url);
                if(sprayDecals[userID].GetTexture("Base_Map") == null)
                    sprayDecals[userID].SetTexture("Base_Map", defaultSpray);
            }
        }
    }

    public void ReloadSprays()
    {
        InvokeRemoteMethod(nameof(SynchedSetUserSpray), UserId.All, Multiplayer.Instance.Me.Index);
    }

    [SynchronizableMethod]
    public void CommitTheft(int UserID)
    {
        InvokeRemoteMethod(nameof(SynchedSetUserSpray), UserId.AllInclusive, Multiplayer.Instance.Me.Index, localImageURL);
    }

}
