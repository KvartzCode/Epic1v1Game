using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeMenu : MonoBehaviour
{
    [SerializeField] GameObject hatPos;

    GameObject currentHat;

    int currentIntex = 0;
    int hatAmmount;

    private void Start()
    {
        hatAmmount = CosmeticManager.Instance.GetHatAmount();
    }

    public void UpdateIndex(bool add)
    {
        currentIntex = add ? ((currentIntex + 1) >= hatAmmount ? 0 : currentIntex + 1) : ((currentIntex - 1) < 0 ? hatAmmount - 1 : currentIntex - 1);
        UpdateHat();
    }

    void UpdateHat()
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(CosmeticManager.Instance.GetHat(currentIntex), hatPos.transform);

        CosmeticManager.Instance.UpdateCurrentHat(currentIntex);
    }


}
