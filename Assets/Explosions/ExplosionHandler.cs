using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class ExplosionHandler : AttributesSync
{
    public Transform ExploSpawner;
    public GameObject Explo;
    public GameObject Rocket;
    public GameObject ExploCosmetic;
    private GameObject exploObject;
    [SerializeField] GameObject railGunPrefab;
    [SerializeField] private List<GameObject> ExplosionVariations;
    [SerializeField] private List<AudioClip> RandomHitSFX;
    [SerializeField] private List<AudioClip> SpecificSFX;
    public static ExplosionHandler Instance;
    public User user;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Debug.LogError("MORE THAN ONE EXPLOSION HANDLER");

    }

    void Initialize()
    {
        user = GameManager.Instance.user;
        Debug.Log(user.Index);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnExplosion(5, 500, 10, 0, ExploSpawner.position, user.Index);
        }
    }


    public void SpawnRocket(Vector3 PositionInWorldSpace, Vector3 dir)
    {
        if (user == null)
            Initialize();

        InvokeRemoteMethod(nameof(SpawnRocketSynchronizable), UserId.AllInclusive, PositionInWorldSpace, dir, user.Index);
        //SpawnRocketSynchronizable(PositionInWorldSpace, dir);
    }

    [SynchronizableMethod]
    void SpawnRocketSynchronizable(Vector3 PositionInWorldSpace, Vector3 dir, int playerId)
    {
        var rocketObj = Instantiate(Rocket, PositionInWorldSpace, Rocket.transform.rotation);
        rocketObj.transform.forward = dir;
        rocketObj.GetComponent<Rocket>().playerID = playerId;

    }

    public void SpawnExplosion(float explosionDamage, float knockbackPower, float explosionRadius, int explosionType, Vector3 PositionInWorldSpace, int userID)
    {
        if (user == null)
            Initialize();

        InvokeRemoteMethod(nameof(SpawnExplosionSynchronizable), UserId.AllInclusive, explosionDamage, knockbackPower, explosionRadius, explosionType, PositionInWorldSpace, userID);
        //SpawnExplosionSynchronizable(explosionDamage, knockbackPower, explosionRadius, explosionType, PositionInWorldSpace);
    }


    public void SpawnLocalExplosion(float explosionDamage, float knockbackPower, float explosionRadius, int explosionType, int fireUserID, Vector3 hitOffset, int hitUserID)
    {
        if (user == null)
            Initialize();

        InvokeRemoteMethod(nameof(SpawnLocalExplosionSynchronizable), UserId.AllInclusive, explosionDamage, knockbackPower, explosionRadius, explosionType, fireUserID, hitOffset, hitUserID);
        //SpawnExplosionSynchronizable(explosionDamage, knockbackPower, explosionRadius, explosionType, PositionInWorldSpace);
    }

    public void SpawnSFX(Vector3 PositionInWorldSpace)
    {
        InvokeRemoteMethod(nameof(SpawnSFXSync), UserId.AllInclusive, Random.Range(0, RandomHitSFX.Count), PositionInWorldSpace);
    }
    public void SpawnSpecificSFX(Vector3 PositionInWorldSpace, int ID)
    {
        InvokeRemoteMethod(nameof(SpawnSpecificSFXSync), UserId.AllInclusive, ID, PositionInWorldSpace);
    }

    public void SpawnRailGunSfx(Vector3 startPos, Vector3 endPos)
    {
        InvokeRemoteMethod(nameof(SpawnRailGunSfxSyncronizable), UserId.AllInclusive, startPos, endPos);
    }

    [SynchronizableMethod]
    void SpawnRailGunSfxSyncronizable(Vector3 startPos, Vector3 endPos)
    {
        GameObject sfx = Instantiate(railGunPrefab, startPos, railGunPrefab.transform.rotation);
        sfx.GetComponent<LineRenderer>().SetPosition(0, startPos);
        sfx.GetComponent<LineRenderer>().SetPosition(1, endPos);
        Destroy(sfx, 4);
    }

    [SynchronizableMethod]
    void SpawnExplosionSynchronizable(float explosionDamage, float knockbackPower, float explosionRadius, int explosionType, Vector3 PositionInWorldSpace, int userID)
    {
        //ExploSpawner.position = PositionInWorldSpace;
        exploObject = Instantiate(Explo, PositionInWorldSpace, Explo.transform.rotation, ExploSpawner);
        exploObject.GetComponent<Explosion>().InitiateExpo(explosionDamage, knockbackPower, explosionRadius, userID);

        Instantiate(ExplosionVariations[explosionType], PositionInWorldSpace, ExplosionVariations[explosionType].transform.rotation, ExploSpawner);
    }

    [SynchronizableMethod]
    void SpawnLocalExplosionSynchronizable(float explosionDamage, float knockbackPower, float explosionRadius, int explosionType, int fireUserID, Vector3 hitOffset, int hitUserID)
    {
        ushort index = System.Convert.ToUInt16(hitUserID);
        Vector3 pos = Multiplayer.Instance.GetAvatar(index).transform.position + hitOffset;
        exploObject = Instantiate(Explo, pos, Explo.transform.rotation, ExploSpawner);
        exploObject.GetComponent<Explosion>().InitiateExpo(explosionDamage, knockbackPower, explosionRadius, fireUserID);

        Instantiate(ExplosionVariations[explosionType], pos, ExplosionVariations[explosionType].transform.rotation, ExploSpawner);
    }

    [SynchronizableMethod]

    void SpawnSFXSync(int SfxID, Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(RandomHitSFX[SfxID], pos);
    }

    [SynchronizableMethod]

    void SpawnSpecificSFXSync(int SfxID, Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(SpecificSFX[SfxID], pos);
    }
}
