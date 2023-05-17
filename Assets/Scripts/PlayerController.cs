using System.Collections;
using System.Collections.Generic;
using Alteruna;
using Fragsurf.Movement;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Alteruna.Avatar))]
public class PlayerController : AttributesSync
{
    public int Stocks { get; private set; } = 4;

    private Alteruna.Avatar avatar;

    [SerializeField] Transform cameraHolder;
    private OriginalCameraStats originalCameraStats;

    [SerializeField] SurfCharacter surfCharacter;

    bool isDead = false;


    void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
    }

    void Start()
    {
        if (!avatar.IsMe)
        {
            enabled = false;
            return;
        }

        GameManager.Instance.SetUser(avatar.Possessor);
        Init();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Debug.Log(GameManager.Instance.user.Index);
        //    Debug.Log("Host ID = " + Multiplayer.Instance.GetUser(Multiplayer.Instance.LowestUserIndex).Index);
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //    GameManager.LeaveRoom();
    }


    void Init()
    {
        originalCameraStats = new OriginalCameraStats(Camera.main.transform);
        Camera.main.transform.SetParent(cameraHolder);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.rotation = cameraHolder.rotation;
        Multiplayer.Instance.RoomLeft.AddListener(RevertCamera);

        //hud = Instantiate(PlayerHudToSpawn).GetComponent<PlayerHud>();
    }

    public void RevertCamera(Multiplayer multiplayer)
    {
        Camera.main.transform.parent = null;
        Camera.main.transform.localPosition = originalCameraStats.position;
        Camera.main.transform.rotation = originalCameraStats.rotation;
    }


    private void Respawn()
    {
        //TODO: Disable player velocity
        surfCharacter.SetMultiplier(0);
        gameObject.transform.position = Multiplayer.AvatarSpawnLocation.position;
    }

    private void HidePlayer(bool hide)
    {
        //foreach (var c in componentsToHide)
        //{
        //    //c.enabled = !hide;
        //}
    }

    private IEnumerator TriggerDeath()
    {
        isDead = true;

        //if (Stocks > 0) //TODO: Uncomment when we fix this
        //{
            Stocks--;
            GameManager.Instance.hud.SetStocks(Stocks);
            HidePlayer(true);

            yield return new WaitForSeconds(5);

            HidePlayer(false);
            Respawn();
            isDead = false;
            
        //}
        //else
        //{
        //    HidePlayer(true);
        //    //TODO: DEATH
        //}
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone") && !isDead)
            StartCoroutine(TriggerDeath());
    }

    private new void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(RevertCamera);
        base.OnDestroy();
    }


    private class OriginalCameraStats
    {
        public Vector3 position;
        public Quaternion rotation;

        public OriginalCameraStats(Transform transform)
        {
            position = transform.position;
            rotation = transform.rotation;
        }
    }
}
