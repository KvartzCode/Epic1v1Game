using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public Transform directionTransform;
    public GameObject projectilePrefab;
    public float launchPower = 20f;
    public float gravity = 15;
    public List<Projectile> currentGrenades;
    [SerializeField] ExplosionHandler explo;

    //debug
    bool hasshot;

    private void Awake()
    {
        currentGrenades = new List<Projectile>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (hasshot)
            {
                Explode(0);
                hasshot = false;
            }
            else
            {
                Launch(0, 0, directionTransform.position, directionTransform.rotation.eulerAngles);
                hasshot = true;
            }

        }
    }

    public void Launch(int playerID, int explosionType, Vector3 position, Vector3 direction)
    {

        GameObject projectileInstance = Instantiate(projectilePrefab, directionTransform.position, directionTransform.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();


        if (projectileScript != null)
        {
            projectileScript.gravity = gravity;
            Vector3 launchVelocity = directionTransform.forward * launchPower;
            projectileScript.Launch(playerID, explosionType, launchVelocity);
            currentGrenades.Add(projectileScript);
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Projectile script attached!");
        }
    }

    public void Explode(int playerID)
    {

        for (int i = currentGrenades.Count - 1; i >= 0; i--)
        {

            //if (playerID == realplayerID)
            //{
            ExplosionHandler.Instance.SpawnExplosion(30, 800, 7, currentGrenades[i].explosionType, currentGrenades[i].transform.position);
            //}

            if (playerID == currentGrenades[i].playerID)
            {
                currentGrenades[i].Delete();
                currentGrenades.Remove(currentGrenades[i]);
            }
        }


    }
}

