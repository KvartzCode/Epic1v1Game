using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "ScriptableObjects/Level")]
public class Level : ScriptableObject
{
    public string Name;
    public Texture2D Image;
    public GameObject Prefab;
    public GameModeType[] AvailableGamemodes;
}
