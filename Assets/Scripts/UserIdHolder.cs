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

    public void SetMultiplier(float multiplier, int id)
    {
        InvokeRemoteMethod(nameof(UpdateMultiplier), UserId.AllInclusive, multiplier,id);
    }


    [SynchronizableMethod]
    private void UpdateMultiplier(float multiplier, int id)
    {
        if (userId == id)
        {
            var multi = multiplier * 100f;
            _multiplier = Mathf.FloorToInt(multi);
            text.text = _multiplier + "%";
        }
    }
    public int GetUserId()
    {
        return userId;
    }

    [SynchronizableMethod]
    private void UpdateUserId(int id)
    {
        Debug.Log("I WANT TO CHANGE to id:  " + id, this); ;
        userId = id;
        Debug.Log("Changed to " + userId);

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
