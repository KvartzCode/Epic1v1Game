using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CustomizeMenu : MonoBehaviour
{
    [SerializeField] GameObject hatPos;
    [SerializeField] TextMeshProUGUI sprayStatusText;

    GameObject currentHat;

    int currentIntex = 0;
    int hatAmmount;
    string tmpSprayUrl;

    private void Start()
    {
        hatAmmount = CosmeticManager.Instance.GetHatAmount();
    }

    public void UpdateIndex(bool add)
    {
        currentIntex = add ? ((currentIntex + 1) >= hatAmmount ? 0 : currentIntex + 1) : ((currentIntex - 1) < 0 ? hatAmmount - 1 : currentIntex - 1);
        UpdateHat();
    }

    public void UpdateUrl(string url)
    {
        sprayStatusText.text = "";
        tmpSprayUrl = url;
    }

    public void SetSpray()
    {
        StartCoroutine(LoadTextureFromURL());

    }

    private IEnumerator LoadTextureFromURL()
    {
        sprayStatusText.text = "";
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(tmpSprayUrl);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Web request error: " + www.error);
            sprayStatusText.text = "<color=#ff0000ff>ERROR: " + www.error + "</color>";
        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            if (myTexture != null)
            {
                Debug.Log("Successfully loaded texture from " + tmpSprayUrl + ", size: " + myTexture.width + "x" + myTexture.height);
                sprayStatusText.text = "<color=#00ff00ff>Spray Loaded!</color>";
                SprayHandler.Instance.SetSpray(tmpSprayUrl);
            }
            else
            {
                Debug.Log("Failed to load texture from " + tmpSprayUrl);
                sprayStatusText.text = "<color=#ff0000ff>Failed to load texture from url</color>";
            }
        }
    }

    void UpdateHat()
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(CosmeticManager.Instance.GetHat(currentIntex), hatPos.transform);

        CosmeticManager.Instance.UpdateCurrentHat(currentIntex);
    }


}
