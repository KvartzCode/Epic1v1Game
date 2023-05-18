using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Networking;
using Alteruna;

public class SprayHandler : AttributesSync
{
    [SerializeField] private List<Material> sprayDecals;

    void Start()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            InvokeRemoteMethod(nameof(SetUserSpray), UserId.AllInclusive, Multiplayer.Instance.Me.Index, "https://cdn.discordapp.com/attachments/1072154976601772083/1108799510773243915/pepsi.png");
        }
    }

    [SynchronizableMethod]
    public void SetUserSpray(int userID, string url)
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
            }
        }
    }

    public void ReloadSprays()
    {
        InvokeRemoteMethod(nameof(SetUserSpray), UserId.All, Multiplayer.Instance.Me.Index);
    }

    [SynchronizableMethod]
    public void CommitTheft(int UserID)
    {
        InvokeRemoteMethod(nameof(SetUserSpray), UserId.AllInclusive, Multiplayer.Instance.Me.Index, "https://cdn.discordapp.com/attachments/1072154976601772083/1108799510773243915/pepsi.png");
    }

}
