using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionShootTest : MonoBehaviour
{
    [SerializeField] ExplosionHandler explo;
    [SerializeField] float raycastDistance = 100f; // Set the distance of the raycast

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            int layerMask = 1 << LayerMask.NameToLayer("Player");
            layerMask = ~layerMask;

            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                Vector3 hitPosition = hit.point;
                explo.SpawnExplosion(10, 3000, 20, 0, hitPosition);
            }
            else //if the raycast doesn't hit anything, the explosion will happen at the end of the raycast distance
            {
                Vector3 endRayPosition = ray.GetPoint(raycastDistance);
                explo.SpawnExplosion(10, 500, 10, 0, endRayPosition);
            }
        }
    }
}
