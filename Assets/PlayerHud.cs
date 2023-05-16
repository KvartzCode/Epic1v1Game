using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private Sprite Red;
    [SerializeField] private Sprite Blue;
    [SerializeField] private Sprite Life;
    [SerializeField] private Image HudColor;
    [SerializeField] private Image stock1;
    [SerializeField] private Image stock2;
    [SerializeField] private Image stock3;
    [SerializeField] private TextMeshProUGUI Procentile;
    public bool isRed;
    // Start is called before the first frame update

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        if (isRed)
        {
            HudColor.sprite = Red;
        }
        else
        {
            HudColor.sprite = Blue;
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
