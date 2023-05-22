using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class CustomizeMenu : MonoBehaviour
{
    [SerializeField] GameObject hatPos;
    [SerializeField] GameObject resetButton;
    [SerializeField] TextMeshProUGUI sprayStatusText;
    [SerializeField] TMP_InputField inputField;

    GameObject currentHat;

    int currentIntex = 0;
    int hatAmmount;
    string tmpSprayUrl;

    private void Start()
    {
        hatAmmount = CosmeticManager.Instance.GetHatAmount();

        if (PlayerPrefs.HasKey(PlayerPrefsHolder.HATKEY))
        {
            currentIntex = PlayerPrefs.GetInt(PlayerPrefsHolder.HATKEY);
            UpdateHat();
        }

        if (PlayerPrefs.HasKey(PlayerPrefsHolder.SPRAYKEY) && PlayerPrefs.GetString(PlayerPrefsHolder.SPRAYKEY) != "")
        {
            resetButton.SetActive(true);
            tmpSprayUrl = PlayerPrefs.GetString(PlayerPrefsHolder.SPRAYKEY);
            SetSpray();
        }
        else
            resetButton.SetActive(false);
    }

    public void UpdateIndex(bool add)
    {
        currentIntex = add ? ((currentIntex + 1) >= hatAmmount ? 0 : currentIntex + 1) : ((currentIntex - 1) < 0 ? hatAmmount - 1 : currentIntex - 1);
        PlayerPrefs.SetInt(PlayerPrefsHolder.HATKEY, currentIntex);
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

    public void ResetSpray()
    {
        resetButton.SetActive(false);
        tmpSprayUrl = "";
        inputField.text = "";
        PlayerPrefs.DeleteKey(PlayerPrefsHolder.SPRAYKEY);
        sprayStatusText.text = "<color=#00ff00ff>Spray Reset!</color>";
        SprayHandler.Instance.SetSpray(tmpSprayUrl);
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
                resetButton.SetActive(true);
                PlayerPrefs.SetString(PlayerPrefsHolder.SPRAYKEY, tmpSprayUrl);
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
