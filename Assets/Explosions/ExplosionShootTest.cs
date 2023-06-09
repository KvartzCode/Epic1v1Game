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


    float railgunForce = 40f;

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

                int layerMask = (1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("LocalPlayer"));
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
        GameManager.Instance.hud.StartRailGunBar(railGunCooldown);
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
            PlayerCol pCol = hit.collider.GetComponent<PlayerCol>();
            if (pCol != null)
            {
                int index = pCol.player.GetComponent<UserIdHolder>().GetUserId();

                GameManager.Instance.AddForceOnPlayer(System.Convert.ToUInt16(index), dir, railgunForce, true, true, 0.5f);

                if (GameManager.Instance.CheckKO(dir, pCol.player.gameObject, railgunForce * 0.7f, false))
                {
                    GameManager.Instance.audioManager.PlayGlobal3DSoundEffect(2, 1f, 100, transform.position);
                    GameManager.Instance.audioManager.Play2DSoundEffectForSpecificPlayer(2, .4f, 100, index);
                }
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

