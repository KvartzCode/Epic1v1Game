using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteAfterSpawn : MonoBehaviour
{
    public float time;
    private void Start()
    {
        Invoke("KillMe", time);
    }

    public void KillMe()
    {
        Destroy(gameObject);
    }
}
