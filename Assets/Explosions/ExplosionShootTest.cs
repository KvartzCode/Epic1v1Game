using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionShootTest : MonoBehaviour
{
    //[SerializeField] ExplosionHandler explo;
    [SerializeField] Animator rocketLauncherAnim;
    [SerializeField] Fragsurf.Movement.SurfCharacter player;
    [SerializeField] Transform spawnPos;
    [SerializeField] AudioClip clip;
    [SerializeField] float cooldown = 0.4f;
    [SerializeField] float sprayDistance = 2f; // Set the distance of the raycast

    AudioSource source;
    Vector3 localSpawnPos;
    float timer;
    bool canShoot = true;

    GameObject debug;

    private void Start()
    {
        if (!player.GetAvatar().IsMe)
        {
            enabled = false;
            return;
        }

        debug = new GameObject("DEBUG");
        debug.transform.parent = transform;

        source = GetComponent<AudioSource>();
        localSpawnPos = spawnPos.transform.localPosition;
    }

    public void SetCanShoot(bool value)
    {
        canShoot = value;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (canShoot)
        {

            if (Input.GetKey(KeyCode.Mouse0) && timer > cooldown)
            {
                timer = 0;
                rocketLauncherAnim.SetTrigger("Fire");
                source.PlayOneShot(clip);


                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;


                int layerMask = 1 << LayerMask.NameToLayer("Rocket");
                layerMask = ~layerMask;
                Vector3 endRayPosition;
                if (Physics.Raycast(ray, out hit, 1000, layerMask,QueryTriggerInteraction.Ignore))
                {
                    endRayPosition = hit.point;
                }
                else //if the raycast doesn't hit anything, the explosion will happen at the end of the raycast distance
                {
                    endRayPosition = ray.GetPoint(1000);
                }


                Debug.Log(hit.collider.gameObject.name);

                debug.transform.localPosition = (transform.forward * 10);
                Vector3 dir = Vector3.Distance(transform.position, endRayPosition) >= 1.25f ? (endRayPosition - spawnPos.position) : transform.forward;
                ExplosionHandler.Instance.SpawnRocket(spawnPos.position, dir);

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

            if (Input.GetKeyDown(KeyCode.T))
            {

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                int layerMask = 1 << LayerMask.NameToLayer("Player");
                layerMask = ~layerMask;

                if (Physics.Raycast(ray, out hit, sprayDistance, layerMask))
                {

                    SprayHandler.Instance.Spray(GameManager.Instance.user.Index, hit.point, -hit.normal);
                }


            }
        }
    }
}

