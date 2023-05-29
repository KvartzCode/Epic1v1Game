using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Alteruna;

public class UserIdHolder : AttributesSync
{
    [SerializeField] TextMeshPro text;
    [Header("PlayerModel")]
    public GameObject playerModel;
    public GameObject hatPos;
    public Material shadowcasterMat;


    private int userId;
    private int _multiplier;
    GameObject camObj;
    GameObject currentHat;
    bool updateText;
    bool hasChangedColor;
    bool haveCheckedISPlayer;
    bool hasUpdatedMaterial;

    int currentHatID;
    Alteruna.Avatar _avatar;

    public void SetUserId(int id)
    {
        InvokeRemoteMethod(nameof(UpdateUserId), UserId.AllInclusive, id);
    }

    public void SetHat(int hatID)
    {
        InvokeRemoteMethod(nameof(UpdateHat), UserId.AllInclusive, hatID);
    }

    [SynchronizableMethod]
    void UpdateHat(int hatID)
    {
        if (currentHat != null)
            Destroy(currentHat);

        currentHat = Instantiate(CosmeticManager.Instance.GetHat(hatID), hatPos.transform);

        currentHatID = hatID;

        if (userId == GameManager.Instance.user.Index)
        {
            MeshRenderer[] renderers = currentHat.GetComponentsInChildren<MeshRenderer>();
            for (int x = 0; x < renderers.Length; x++)
            {

                Material[] mats = renderers[x].materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = shadowcasterMat;
                }
                renderers[x].materials = mats;
            }

            SkinnedMeshRenderer[] sRenderer = currentHat.GetComponentsInChildren<SkinnedMeshRenderer>();
            for (int x = 0; x < sRenderer.Length; x++)
            {
                Material[] mats = sRenderer[x].materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = shadowcasterMat;
                }
                sRenderer[x].materials = mats;


            }


        }

    }

    public int GetCurrentHatIndex()
    {
        return currentHatID;
    }

    public void UpdatePlayerMat()
    {
        if (userId != GameManager.Instance.user.Index)
        {
            Material[] mat = playerModel.GetComponent<MeshRenderer>().materials;
            mat[0] = CosmeticManager.Instance.GetPlayerMat(userId);
            playerModel.GetComponent<MeshRenderer>().materials = mat;
        }
    }

    public void DisableText()
    {
        text.enabled = false;
        updateText = false;
    }

    private void Start()
    {
        _avatar = GetComponent<Alteruna.Avatar>();

        if (_avatar.IsMe)
        {
            gameObject.tag = "Player";

            if (playerModel != null && shadowcasterMat != null)
            {
                Material[] mats = playerModel.GetComponent<MeshRenderer>().materials;
                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = shadowcasterMat;
                }
                playerModel.GetComponent<MeshRenderer>().materials = mats;
            }

        }

        MusicPlayer.Instance.SynchAll();

        camObj = Camera.main.transform.gameObject;
    }

    private void Update()
    {
        if (updateText)
            text.gameObject.transform.LookAt(GameManager.Instance.GetListener());
    }

    public void SetMultiplier(float multiplier, int id)
    {
        InvokeRemoteMethod(nameof(UpdateMultiplier), UserId.AllInclusive, multiplier, id);
    }

    public float GetMultiplier()
    {
        return _multiplier;
    }

    [SynchronizableMethod]
    private void UpdateMultiplier(float multiplier, int id)
    {

        if (userId == id)
        {
            if (!hasChangedColor)
            {
                text.color = GameManager.Instance.hud.GetColor(id);
                hasChangedColor = true;
            }

            var multi = multiplier * 100f;
            _multiplier = Mathf.FloorToInt(multi);
            text.text = _multiplier + "%";

            if (userId == GameManager.Instance.user.Index)
            {
                GameManager.Instance.hud.UpdatePercent(_multiplier);
            }
        }
    }

    public int GetUserId()
    {
        return userId;
    }

    [SynchronizableMethod]
    private void UpdateUserId(int id)
    {
        updateText = true;
        Debug.Log("I WANT TO CHANGE to id:  " + id, this); ;
        userId = id;
        Debug.Log("Changed to " + userId);

        if (!hasUpdatedMaterial)
        {
            hasUpdatedMaterial = true;
            UpdatePlayerMat();
        }

        if (!haveCheckedISPlayer)
        {

            if (!hasChangedColor)
            {
                text.color = GameManager.Instance.hud.GetColor(id);
                hasChangedColor = true;
                GameManager.Instance.SynchedUpdateAllMultipliers();
            }

            haveCheckedISPlayer = true;
            if (userId == GameManager.Instance.user.Index)
            {
                DisableText();
            }
        }

    }


}
