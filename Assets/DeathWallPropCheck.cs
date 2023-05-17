using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWallPropCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 1)
        {
            other.GetComponent<PropRespawn>().ResetProp();
        }
    }
}
