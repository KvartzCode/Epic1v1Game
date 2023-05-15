using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRidgidbody : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Fragsurf.Movement.SurfCharacter>().AddVelocity(other.transform.position - transform.position, 10);
        }

    }
}
