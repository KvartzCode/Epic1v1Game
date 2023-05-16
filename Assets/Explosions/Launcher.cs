using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class Launcher : MonoBehaviour
{
    [SerializeField] public Fragsurf.Movement.SurfCharacter player;
    public Transform directionTransform;
    public GameObject projectilePrefab;
    public float launchPower = 20f;
    public float gravity = 15;
    public List<Projectile> currentGrenades;
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
                ExplodeGrenade(Multiplayer.Instance.Me.Index);
                hasshot = false;
            }
            else
            {
                LaunchGrenade(Multiplayer.Instance.Me.Index, 0, directionTransform.position, directionTransform.rotation.eulerAngles,player.moveData.velocity);
                hasshot = true;
            }

        }
    }

    public void LaunchGrenade(int playerID, int explosionType, Vector3 position, Vector3 direction,Vector3 PlayerVeclocity)
    {

        GameObject projectileInstance = Instantiate(projectilePrefab, directionTransform.position, directionTransform.rotation);
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();


        if (projectileScript != null)
        {
            projectileScript.gravity = gravity;
            Vector3 launchVelocity = directionTransform.forward * launchPower + PlayerVeclocity;
            projectileScript.Launch(playerID, explosionType, launchVelocity);
            currentGrenades.Add(projectileScript);
        }
        else
        {
            Debug.LogError("Projectile prefab does not have a Projectile script attached!");
        }
    }

    public void ExplodeGrenade(int playerID)
    {

        for (int i = currentGrenades.Count - 1; i >= 0; i--)
        {

            if (playerID == Multiplayer.Instance.Me.Index)
            {
                ExplosionHandler.Instance.SpawnExplosion(30, 800, 7, currentGrenades[i].explosionType, currentGrenades[i].transform.position,playerID);
            }

            if (playerID == currentGrenades[i].playerID)
            {
                currentGrenades[i].Delete();
                currentGrenades.Remove(currentGrenades[i]);
            }
        }


    }
}

