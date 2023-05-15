using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum ExplosionType
    {
        Small,
        Big,
        Slime
    };
    [Tooltip("What particle/audio prefab to use for the explosion")]
    public ExplosionType ExplType;

    public void InitiateExpo(float explosionDamage, float explosionPower, float explosionRadius)
    {
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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ColliderFound");

        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log(other + " has rigidbody");
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
            Debug.Log("Applied force");
        }
        else
        {
            Debug.Log(other + " does not have rigidbody");
        }

        //Use this space for the player calculations
        //No clue how to do that yet though...

    }



    private void deleteSelf()
    {
        Destroy(gameObject);
    }
}
