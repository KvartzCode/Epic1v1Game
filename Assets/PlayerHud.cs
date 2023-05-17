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

    public void LoseStock()
    {
        if (stock3.sprite != null)
        {
            stock3.sprite = null;
            return;
        }
        else if (stock2.sprite != null)
        {
            stock2.sprite = null;
            return;
        }
        else if (stock1.sprite != null)
        {
            stock1.sprite = null;
            return;
        }
        else if (stock1.sprite == null)
        {
            Debug.Log("Game over!");
            return;
        }
    }
}
