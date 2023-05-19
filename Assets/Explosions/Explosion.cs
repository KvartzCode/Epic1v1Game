using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class Explosion : MonoBehaviour
{


    [Tooltip("Explosion Max Damage")]
    public float explosionDamage;
    [Tooltip("Explosion Knockback Power")]
    public float explosionPower;
    [Tooltip("Explosion Radius Size")]
    public float explosionRadius;
    private SphereCollider sphereCol;
    private Rigidbody rg;

    int _userID;

    public enum ExplosionType
    {
        Small,
        Big,
        Slime
    };
    [Tooltip("What particle/audio prefab to use for the explosion")]
    public ExplosionType ExplType;

    public void InitiateExpo(float explosionDamage, float explosionPower, float explosionRadius, int userId)
    {
        _userID = userId;
        this.explosionDamage = explosionDamage;
        this.explosionPower = explosionPower;
        this.explosionRadius = explosionRadius;
        sphereCol = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        sphereCol.radius = explosionRadius;
        sphereCol.isTrigger = true;
        rg = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        rg.isKinematic = true;

        Invoke("deleteSelf", 0.2f);
    }

    private bool CheckKO(Vector3 direction, Collider other, float force)
    {
        Debug.Log("Entered CheckKO method.");

        float threshhold = 30;
        Vector3 pos = other.transform.position;
        float multiplier = other.GetComponent<Fragsurf.Movement.SurfCharacter>().GetMultiplier();

        Debug.Log("Force multiplier: " + (force * multiplier).ToString());

        if (force * multiplier < threshhold)
        {
            Debug.Log("Force times multiplier is less than threshold. Returning false.");
            return false;
        }
        else
        {
            int layerMask = 1 << 3;
            layerMask = ~layerMask; // invert to ignore layer 3

            RaycastHit hit;
            if (Physics.Raycast(pos, direction, out hit, Mathf.Infinity, layerMask))
            {
                Debug.Log("Raycast hit an object: " + hit.collider.gameObject.name);
                // Check if the first object the raycast hit is a trigger and has the tag "DeathZone"
                if (hit.collider.isTrigger && hit.collider.tag == "DeathZone")
                {
                    Debug.Log("Hit object is a trigger and tagged as 'DeathZone'. Returning true.");
                    other.GetComponent<Fragsurf.Movement.SurfCharacter>().SetVelocity(Vector3.zero);
                    ExplosionHandler.Instance.SpawnSpecificSFX(pos, 0);
                    GameManager.Instance.SetTimeScale(0.01f, 0.01f, true);
                    return true;
                }
                else
                {
                    Debug.Log("Hit object is either not a trigger or not tagged as 'DeathZone'. Returning false.");
                }
            }
            else
            {
                Debug.Log("No raycast hit detected. Returning false.");
            }
        }

        return false;
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<Fragsurf.Movement.SurfCharacter>() != null)
        {
            Vector3 direction = (other.transform.position + new Vector3(0, 1, 0)) - transform.position; // calculate the vector between the two positions
            float distance = direction.magnitude; // calculate the distance

            // Normalize the vector to get the direction and add 0.2f to the Y component
            direction = direction.normalized;
            direction.y += 0;

            // Scale the force by the inverse of the ratio of the distance to the explosionRadius
            // The force will be maximum when the distance is 0 and minimum when the distance equals explosionRadius
            float forceScale = 1 - Mathf.Clamp01(distance / explosionRadius);

            //Deal damage
            if (_userID != GameManager.Instance.user.Index)
            {

                other.GetComponent<Fragsurf.Movement.SurfCharacter>().AddDamage(10 - Vector3.Distance(transform.position, other.transform.position) / 2);
            }

            //(direction * explosionPower * forceScale) * //Playerhealth;
            if(_userID != GameManager.Instance.user.Index)
            if (CheckKO(direction, other, explosionPower * forceScale * 0.05f))
            {
                explosionPower = explosionPower * 3;
            }
   
            other.GetComponent<Fragsurf.Movement.SurfCharacter>().AddVelocity(direction, explosionPower * forceScale * 0.05f, _userID == GameManager.Instance.user.Index ? false : true);

            return;
        }

        if (other.GetComponent<Rigidbody>() != null)
        {
            Rigidbody riggy = other.GetComponent<Rigidbody>();

            Vector3 direction = other.transform.position - transform.position; // calculate the vector between the two positions
            float distance = direction.magnitude; // calculate the distance

            // Normalize the vector to get the direction and add 0.2f to the Y component
            direction = direction.normalized;
            direction.y += 1;

            // Scale the force by the inverse of the ratio of the distance to the explosionRadius
            // The force will be maximum when the distance is 0 and minimum when the distance equals explosionRadius
            float forceScale = 1 - Mathf.Clamp01(distance / explosionRadius);

            riggy.AddForce(direction * explosionPower * forceScale); // apply the force
            return;
        }
        else
        {

        }

    }

    public void GetDir()
    {

    }



    private void deleteSelf()
    {
        Destroy(gameObject);
    }
}
