using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTrigger : MonoBehaviour
{
    [SerializeField] Rocket rocket;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            UserIdHolder idHolder = other.gameObject.GetComponent<UserIdHolder>();
            if (idHolder != null)
                if (idHolder.GetUserId() != rocket.playerID)
                    rocket.Explode();
        }
    }
}
