using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Fragsurf.Movement.SurfCharacter>().AddVelocity(other.transform.forward + (transform.up * 3), 10, false);
        }
    }
}
