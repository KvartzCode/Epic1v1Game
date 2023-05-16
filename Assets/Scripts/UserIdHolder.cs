using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class UserIdHolder : AttributesSync
{
    private int userId;

    public void SetUserId(int id)
    {
        InvokeRemoteMethod(nameof(UpdateUserId), UserId.AllInclusive, id);
    }

    public int GetUserId()
    {
        return userId;
    }

    [SynchronizableMethod]
    private void UpdateUserId(int id)
    {
        userId = id;
    }


}
