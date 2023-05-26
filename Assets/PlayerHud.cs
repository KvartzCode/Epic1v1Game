using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum TeamColor
{
    Red,
    Blue,
    Green,
    Purple,
    Yellow,
    Orange,
    Mint,
    Pink,
}
public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Sprite Red;
    [SerializeField] private Sprite Blue;
    [SerializeField] private Sprite Green;
    [SerializeField] private Sprite Purple;
    [SerializeField] private Sprite Yellow;
    [SerializeField] private Sprite Orange;
    [SerializeField] private Sprite Mint;
    [SerializeField] private Sprite Pink;
    [SerializeField] private Sprite Life;
    [SerializeField] private Color CRed;
    [SerializeField] private Color CBlue;
    [SerializeField] private Color CGreen;
    [SerializeField] private Color CPurple;
    [SerializeField] private Color CYellow;
    [SerializeField] private Color COrange;
    [SerializeField] private Color CMint;
    [SerializeField] private Color CPink;
    [SerializeField] private Image HudColor;
    [SerializeField] private Image stock1;
    [SerializeField] private Image stock2;
    [SerializeField] private Image stock3;
    [SerializeField] private TextMeshProUGUI Procentile;
    public TeamColor color;
    public GameObject[] hudObj;

    bool isDead;
    int currentStocks;

    private void Awake()
    {
        for (int i = 0; i < hudObj.Length; i++)
        {
            hudObj[i].SetActive(false);
        }
    }

    public void StartRound()
    {
        for (int i = 0; i < hudObj.Length; i++)
        {
            hudObj[i].SetActive(true);
        }
        color = (TeamColor)GameManager.Instance.user.Index;
        switch (color)
        {
            case TeamColor.Red:
                HudColor.sprite = Red;
                break;
            case TeamColor.Blue:
                HudColor.sprite = Blue;
                break;
            case TeamColor.Green:
                HudColor.sprite = Green;
                break;
            case TeamColor.Purple:
                HudColor.sprite = Purple;
                break;
            case TeamColor.Yellow:
                HudColor.sprite = Yellow;
                break;
            case TeamColor.Orange:
                HudColor.sprite = Orange;
                break;
            case TeamColor.Mint:
                HudColor.sprite = Mint;
                break;
            case TeamColor.Pink:
                HudColor.sprite = Pink;
                break;
            default:
                Debug.Log("Undefined TeamColor!");
                break;
        }

        stock1.sprite = Life;
        stock2.sprite = Life;
        stock3.sprite = Life;
        SetStocks(4);
    }

    public bool GetIsDead()
    {
        return currentStocks > 0 ? false : true;
    }
    public void RemoveStock()
    {
        SetStocks(currentStocks - 1);
    }

    public Color GetColor(int userId)
    {
        color = (TeamColor)userId;
        switch (color)
        {
            case TeamColor.Red:
                return CRed;

            case TeamColor.Blue:
                return CBlue;

            case TeamColor.Green:
                return CGreen;

            case TeamColor.Purple:
                return CPurple;

            case TeamColor.Yellow:
                return CYellow;

            case TeamColor.Orange:
                return COrange;

            case TeamColor.Mint:
                return CMint;

            case TeamColor.Pink:
                return CPink;

            default:
                Debug.Log("Undefined TeamColor!");
                break;
        }
        return Color.white;
    }

    public void UpdatePercent(int currentPercent)
    {
        Debug.Log(" TEXT SOULD BE: " + currentPercent + "%");
        Procentile.text = currentPercent + "%";
    }

    public void SetStocks(int stocks)
    {
        currentStocks = stocks;
        switch (stocks)
        {
            case 4:
                stock1.enabled = true;
                stock2.enabled = true;
                stock3.enabled = true;
                break;
            case 3:
                stock1.enabled = true;
                stock2.enabled = true;
                stock3.enabled = false;
                break;
            case 2:
                stock1.enabled = true;
                stock2.enabled = false;
                stock3.enabled = false;
                break;
            case 1:
                stock1.enabled = false;
                stock2.enabled = false;
                stock3.enabled = false;
                break;
            case 0:
                break;
            default:
                break;
        }
    }
}
