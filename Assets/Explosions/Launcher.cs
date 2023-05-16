using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Transform directionTransform;
    public GameObject projectilePrefab;
    public float launchPower = 20f;
    public float gravity = 15;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Launch(true); //isowner will be implemented later
        }
    }

    public void Launch(bool isowner) //isowner will be implemented later
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, directionTransform.position, directionTransform.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.gravity = gravity;
            Vector3 launchVelocity = directionTransform.forward * launchPower;
            projectileScript.Launch(launchVelocity);
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Projectile script attached!");
        }
    }
}

