using System.Collections;
using System.Collections.Generic;
using Alteruna;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    private Spawner spawner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SpawnPlayer(int index, Vector3 position, Quaternion rotation = new Quaternion())
    {
        spawner.Spawn(index, position, rotation);
    }

    public void DeSpawnPlayer()
    {

    }
}
