using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;

public class StocksEndScreenManager : MonoBehaviour
{
    [SerializeField] TextMeshPro winnerText;
    [SerializeField] MeshRenderer maxwellRenderer;
    [SerializeField] RawImage renderImage;
    [SerializeField] GameObject hatPos;
    [SerializeField] DecalProjector[] sprays;
    [SerializeField] float startPoint = -20f;
    [SerializeField] float speed = 20;

    GameObject spawnedHat;
    Material mat;
    float currentPoint;

    private void Init()
    {
        mat = renderImage.material;
        mat.SetFloat("_Cutoff_Height", startPoint);
        currentPoint = startPoint;

        Vector4 offset = new Vector4(Random.Range(-500, 500), Random.Range(-500, 500), 0, 0);
        mat.SetVector("_Offset", offset);
    }

    public void SetWinner(int winner)
    {
        Init();
        SetColor(GameManager.Instance.hud.GetColor(winner));
        SpawnHat(winner);
        SetWinnerText(winner);
        SetMat(winner);
        SetSprayMat(winner);
    }

    void SpawnHat(int winner)
    {
        int hatIndex;
        if (winner != GameManager.Instance.user.Index)
        {
            hatIndex = Alteruna.Multiplayer.Instance.GetAvatar(System.Convert.ToUInt16(winner)).GetComponent<UserIdHolder>().GetCurrentHatIndex();
        }
        else
        {
            hatIndex = GameManager.Instance.player.GetComponent<UserIdHolder>().GetCurrentHatIndex();
        }

        if (spawnedHat != null)
            Destroy(spawnedHat);

        spawnedHat = Instantiate(CosmeticManager.Instance.GetHat(hatIndex), hatPos.transform);

    }

    void SetSprayMat(int winner)
    {
        for (int i = 0; i < sprays.Length; i++)
        {
            sprays[i].material = SprayHandler.Instance.GetMat(winner);
        }
    }

    void SetMat(int winner)
    {
        Material[] mats = maxwellRenderer.materials;
        mats[0] = CosmeticManager.Instance.GetPlayerMat(winner);
        maxwellRenderer.materials = mats;
    }

    void SetColor(Color color)
    {
        mat.SetColor("_EdgeColor", color);
    }

    void SetWinnerText(int winner)
    {
        winnerText.text = "Player " + (winner + 1) + " Wins!";
    }

    // Update is called once per frame
    void Update()
    {
        currentPoint += speed * Time.deltaTime;
        mat.SetFloat("_Cutoff_Height", currentPoint);
    }
}
