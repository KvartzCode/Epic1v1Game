using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHandler : MonoBehaviour
{
    public Transform ExploSpawner;
    public GameObject Explo;
    public GameObject ExploCosmetic;
    private GameObject exploObject;
    [SerializeField] private List<GameObject> ExplosionVariations;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnExplosion(5, 500, 10,0,ExploSpawner.position);
        }
    }

    public void SpawnExplosion(float explosionDamage,float knockbackPower,float explosionRadius,int explosionType, Vector3 PositionInWorldSpace)
    {
        ExploSpawner.position = PositionInWorldSpace;
        exploObject = Instantiate(Explo, ExploSpawner);
        exploObject.GetComponent<Explosion>().InitiateExpo(explosionDamage, knockbackPower, explosionRadius);


        exploObject = Instantiate(ExplosionVariations[explosionType], ExploSpawner);
    }
}
