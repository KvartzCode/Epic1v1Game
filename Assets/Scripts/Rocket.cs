using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public int playerID = 0;
    [SerializeField] float speed = 3;

    Rigidbody rbdy;

    private void Start()
    {
        AddForceToRocket();
    }

    public void AddForceToRocket()
    {
        rbdy = GetComponent<Rigidbody>();
        //rbdy.AddForce(transform.forward * speed, ForceMode.Impulse);
        rbdy.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ExplosionHandler.Instance.SpawnExplosion(10, 600, 7, 0, transform.position);
        Destroy(gameObject);
    }
}
