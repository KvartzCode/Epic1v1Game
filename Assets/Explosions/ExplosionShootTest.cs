using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionShootTest : MonoBehaviour
{
    //[SerializeField] ExplosionHandler explo;
    [SerializeField] Animator rocketLauncherAnim;
    [SerializeField] Transform spawnPos;
    [SerializeField] AudioClip clip;
    [SerializeField] float raycastDistance = 100f; // Set the distance of the raycast
    [SerializeField] float cooldown = 0.4f;

    AudioSource source;
    float timer;

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Mouse0) && timer > cooldown)
        {
            timer = 0;
            rocketLauncherAnim.SetTrigger("Fire");
            source.PlayOneShot(clip);
            ExplosionHandler.Instance.SpawnRocket(spawnPos.position, transform.forward);

            //Ray ray = new Ray(transform.position, transform.forward);
            //RaycastHit hit;

            //int layerMask = 1 << LayerMask.NameToLayer("Player");
            //layerMask = ~layerMask;

            //if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            //{
            //    Vector3 hitPosition = hit.point;
            //    explo.SpawnExplosion(10, 600, 7, 0, hitPosition);
            //}
            //else //if the raycast doesn't hit anything, the explosion will happen at the end of the raycast distance
            //{
            //    Vector3 endRayPosition = ray.GetPoint(raycastDistance);
            //    explo.SpawnExplosion(10, 500, 10, 0, endRayPosition);
        }
    }
}

