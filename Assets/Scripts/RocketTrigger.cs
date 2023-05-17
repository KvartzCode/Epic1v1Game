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
            PlayerCol col = other.GetComponent<PlayerCol>();

            if (col != null)
            {
                UserIdHolder idHolder = col.player.gameObject.GetComponent<UserIdHolder>();
                if (idHolder != null)
                {
                    if (idHolder.GetUserId() != rocket.playerID)
                    {
                        rocket.Explode();   
                    }

                }

            }
        }
    }
}
