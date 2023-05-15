using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Is here");
            other.GetComponent<Fragsurf.Movement.SurfCharacter>().AddVelocity(transform.up, 10);
        }
    }
}
