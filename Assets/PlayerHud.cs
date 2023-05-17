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
    Purple
}
public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Sprite Red;
    [SerializeField] private Sprite Blue;
    [SerializeField] private Sprite Green;
    [SerializeField] private Sprite Purple;
    [SerializeField] private Sprite Life;
    [SerializeField] private Image HudColor;
    [SerializeField] private Image stock1;
    [SerializeField] private Image stock2;
    [SerializeField] private Image stock3;
    [SerializeField] private TextMeshProUGUI Procentile;
    public TeamColor color;
    // Start is called before the first frame update

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
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
            default:
                Debug.Log("Undefined TeamColor!");
                break;
        }

        stock1.sprite = Life;
        stock2.sprite = Life;
        stock3.sprite = Life;
    }

    public void UpdatePercent(int currentPercent)
    {
        Procentile.text = currentPercent + "%";
    }

    public void SetStocks(int stocks)
    {
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
            default:
                Debug.Log("Game over!");
                break;
        }
    }
}
