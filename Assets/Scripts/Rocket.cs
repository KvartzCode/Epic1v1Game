using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public int playerID = -10;
    [SerializeField] float speed = 3;
    [SerializeField] ParticleSystem particles;


    Rigidbody rbdy;

    private void Start()
    {
        AddForceToRocket();
        Destroy(gameObject, 10);
    }

    public void AddForceToRocket()
    {
        rbdy = GetComponent<Rigidbody>();
        //rbdy.AddForce(transform.forward * speed, ForceMode.Impulse);
        rbdy.velocity = transform.forward * speed;
    }

    public void Explode()
    {
        if (playerID == GameManager.Instance.user.Index)
            ExplosionHandler.Instance.SpawnExplosion(10, 600, 7, 0, transform.position, playerID);

        DestroyRocket();
    }

    public void ExplodeOnPlayer(int hitId)
    {
        if (playerID == GameManager.Instance.user.Index)
        {
            ushort index = System.Convert.ToUInt16(hitId);
            Vector3 dir = transform.position - Alteruna.Multiplayer.Instance.GetAvatar(index).transform.position;
            ExplosionHandler.Instance.SpawnLocalExplosion(15, 600, 7, 0, playerID, dir, hitId);
        }

        DestroyRocket();

    }

    void DestroyRocket()
    {
        particles.transform.parent = null;
        particles.Stop();
        Destroy(particles.gameObject, 2.5f);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
