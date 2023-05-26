using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Level : MonoBehaviour
{
    public string levelName;
    public Texture2D levelImage;
    public GameObject deathCamPos;
    public List<Transform> spawnPoints;
}