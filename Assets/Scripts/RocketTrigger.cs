using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTrigger : MonoBehaviour
{
    [SerializeField] Rocket rocket;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<UserIdHolder>().GetUserId() != rocket.playerID)
                rocket.Explode();
        }
    }
}
