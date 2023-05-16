using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public static class ExtensionMethods
{
    public static User GetHost(this Multiplayer _m)
    {
        return _m.GetUser(_m.LowestUserIndex);
    }
}
