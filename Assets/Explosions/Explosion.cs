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
            Rigidbody riggy = other.GetComponent<Rigidbody>();
            riggy.AddExplosionForce(explosionPower, transform.position, explosionRadius);
        }
    }

    private void deleteSelf()
    {
        Destroy(gameObject);
    }
}
