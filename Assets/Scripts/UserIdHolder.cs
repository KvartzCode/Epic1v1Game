using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;

public class UserIdHolder : AttributesSync
{
    private int userId;
    private int _multiplier;
    [SerializeField] TextMeshPro text;
    bool uptadeText = true;
    bool haveCheckedISPlayer;

    public void SetUserId(int id)
    {
        InvokeRemoteMethod(nameof(UpdateUserId), UserId.AllInclusive, id);
    }

    public void DisableText()
    {
        text.enabled = false;
        uptadeText = false;
    }

    private void Update()
    {
        if (uptadeText)
            text.gameObject.transform.LookAt(Camera.main.transform);
    }

    public void SetMultiplier(float multiplier)
    {
        InvokeRemoteMethod(nameof(UpdateMultiplier), UserId.AllInclusive, multiplier);
    }


    [SynchronizableMethod]
    private void UpdateMultiplier(float multiplier)
    {
        var multi = multiplier * 100f;
        _multiplier = Mathf.FloorToInt(multi);
        text.text = _multiplier + "%";
    }
    public int GetUserId()
    {
        return userId;
    }

    [SynchronizableMethod]
    private void UpdateUserId(int id)
    {
        userId = id;
        
        if (!haveCheckedISPlayer)
        {
            haveCheckedISPlayer = true;
            if (userId == GameManager.Instance.user.Index)
            {
                DisableText();
            }
        }

    }


}
