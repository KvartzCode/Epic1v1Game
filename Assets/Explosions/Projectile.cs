using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float gravity;
    public bool sentByOwner;
    private Vector3 velocity;
    private Rigidbody rb;
    public int playerID;
    public int explosionType;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    public void Launch(int playerID,int explosionType,Vector3 initialVelocity)
    {
        velocity = initialVelocity;
        enabled = true;
        this.playerID = playerID;
        this.explosionType = explosionType;
    }

    private void FixedUpdate()
    {
        velocity += gravity * Vector3.down * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("The layer is: " + other.gameObject.layer);
        if (other.gameObject.layer == 3)
        {
            if (other.GetComponent<Fragsurf.Movement.SurfCharacter>() != null) //Replace this with playerID
            {
                return;
            }

            return;
        }

        gameObject.GetComponent<SphereCollider>().enabled = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void Delete() //Make sure it explodes the one the player put
    {
        Destroy(gameObject);
    }
}

