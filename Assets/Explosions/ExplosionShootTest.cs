using System;
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
    [SerializeField] float rocketCooldown = 0.4f;
    [SerializeField] float railGunCooldown = 0.4f;
    [SerializeField] float sprayDistance = 4f; // Set the distance of the raycast

    AudioSource source;
    Vector3 localSpawnPos;
    float rocketTimer;
    float railGunTimer;
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
        rocketTimer += Time.deltaTime;
        railGunTimer += Time.deltaTime;
        if (canShoot)
        {

            if (Input.GetKey(KeyCode.Mouse0) && rocketTimer > rocketCooldown)
            {
                SpawnRocket();
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && railGunTimer > railGunCooldown)
            {
                RailGunLogic();
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

    private void RailGunLogic()
    {
        railGunTimer = 0;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Vector3 dir;


        int layerMask = (1 << LayerMask.NameToLayer("Rocket") | 1 << LayerMask.NameToLayer("LocalPlayer"));
        layerMask = ~layerMask;
        Vector3 endRayPosition;
        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            endRayPosition = hit.point;
        }
        else //if the raycast doesn't hit anything, the explosion will happen at the end of the raycast distance
        {
            endRayPosition = ray.GetPoint(1000);
        }


        dir = endRayPosition - transform.position;

        ExplosionHandler.Instance.SpawnRailGunSfx(spawnPos.position, endRayPosition);


        if (hit.collider != null)
        {
            Debug.LogError(hit.collider.gameObject.name, hit.collider);
            if (hit.collider.CompareTag("Player"))
            {
                int index = hit.collider.GetComponent<PlayerCol>().player.GetComponent<UserIdHolder>().GetUserId();

                if (GameManager.Instance.CheckKO(dir, hit.collider, 600, false))
                {
                    GameManager.Instance.audioManager.PlayGlobal3DSoundEffectAtPlayer(2, 1, 100, index);
                }

                GameManager.Instance.AddForceOnPlayer(System.Convert.ToUInt16(index), dir, 600, true, true);

            }
        }


    }

    private void SpawnRocket()
    {
        rocketTimer = 0;
        rocketLauncherAnim.SetTrigger("Fire");
        source.PlayOneShot(clip);


        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;


        int layerMask = 1 << LayerMask.NameToLayer("Rocket");
        layerMask = ~layerMask;
        Vector3 endRayPosition;
        if (Physics.Raycast(ray, out hit, 1000, layerMask, QueryTriggerInteraction.Ignore))
        {
            endRayPosition = hit.point;
        }
        else //if the raycast doesn't hit anything, the explosion will happen at the end of the raycast distance
        {
            endRayPosition = ray.GetPoint(1000);
        }

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
}

