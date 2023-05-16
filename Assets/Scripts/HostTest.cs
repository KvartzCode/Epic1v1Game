using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class HostTest : AttributesSync
{
    [SerializeField] private Alteruna.Avatar _avatar;
    private int clientInfo1 = 3, clientInfo2 = 5;

    private void Start()
    {
        if (!_avatar.IsMe)
        {
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ClientRequestHostLogic();
    }

    void ClientRequestHostLogic()
    {
        InvokeRemoteMethod(nameof(HostBrain), Multiplayer.GetUser(Multiplayer.LowestUserIndex), clientInfo1, clientInfo2);
    }

    [SynchronizableMethod]
    void HostBrain(int userInformation, int moreUserInformation)
    {
        InvokeRemoteMethod(nameof(AllClientsRecieveThis), UserId.AllInclusive, userInformation + moreUserInformation);
    }

    [SynchronizableMethod]
    void AllClientsRecieveThis(int recievedInformation)
    {
        Debug.Log($"All clients recieve this information: {recievedInformation} // This was calculated only on the earliest client");
    }
}