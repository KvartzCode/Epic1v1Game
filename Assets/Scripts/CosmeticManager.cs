using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticManager : MonoBehaviour
{
    [SerializeField] Material defaultPlayerMat;
    [SerializeField] Material player1Mat;
    [SerializeField] Material player2Mat;
    [SerializeField] Material player3Mat;
    [SerializeField] Material player4Mat;
    [SerializeField] GameObject[] hats;

    private int currentHat = 0;

    private static CosmeticManager _instance;
    public static CosmeticManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Debug.LogError("More than one cosmetic manager in scene");
        else
            _instance = this;
    }

    public int GetHatAmount()
    {
        return hats.Length;
    }

    public void UpdateCurrentHat(int index)
    {
        currentHat = index;
        GameManager.Instance.SynchedUpdateHats();
    }

    public int GetCurrentHat()
    {
        return currentHat;
    }

    public Material GetPlayerMat(int id)
    {
        switch (id)
        {
            case 0:
                return player1Mat;
            case 1:
                return player2Mat;
            case 2:
                return player3Mat;
            case 3:
                return player4Mat;
            default:
                return defaultPlayerMat;
        }
    }

    public GameObject GetHat(int index)
    {
        if (index >= 0 && index < hats.Length)
        {
            return hats[index];
        }
        else
            return new GameObject("NO HAT :(");
    }
}
