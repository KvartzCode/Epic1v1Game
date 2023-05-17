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
    [SerializeField] private List<GameObject> ExplosionVariations;
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


    [SynchronizableMethod]
    void SpawnExplosionSynchronizable(float explosionDamage, float knockbackPower, float explosionRadius, int explosionType, Vector3 PositionInWorldSpace, int userID)
    {
        Debug.Log("Explosion");
        //ExploSpawner.position = PositionInWorldSpace;
        exploObject = Instantiate(Explo, PositionInWorldSpace, Explo.transform.rotation, ExploSpawner);
        exploObject.GetComponent<Explosion>().InitiateExpo(explosionDamage, knockbackPower, explosionRadius, userID);

        Instantiate(ExplosionVariations[explosionType], PositionInWorldSpace, ExplosionVariations[explosionType].transform.rotation, ExploSpawner);
    }
}
