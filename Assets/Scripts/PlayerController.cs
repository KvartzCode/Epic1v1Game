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
    GameObject lobbyCanvas;

    bool isDead = false;
    Camera mainCamera;


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
        LockMouse(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F1))
            LockMouse(Cursor.visible);

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
        mainCamera = Camera.main;
        originalCameraStats = new OriginalCameraStats(mainCamera.transform);
        mainCamera.transform.SetParent(cameraHolder);
        mainCamera.transform.localPosition = Vector3.zero;
        mainCamera.transform.rotation = cameraHolder.rotation;
        Multiplayer.Instance.RoomLeft.AddListener(RevertCamera);

        lobbyCanvas = FindObjectOfType<RoomMenu>().gameObject;
        lobbyCanvas.SetActive(false);
        //hud = Instantiate(PlayerHudToSpawn).GetComponent<PlayerHud>();
    }

    public void RevertCamera(Multiplayer multiplayer)
    {
        mainCamera.transform.parent = null;
        mainCamera.transform.localPosition = originalCameraStats.position;
        mainCamera.transform.rotation = originalCameraStats.rotation;
    }


    private void LockMouse(bool isLocked)
    {
        lobbyCanvas.SetActive(!isLocked);
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    private void Respawn()
    {
        //TODO: Disable player velocity
        surfCharacter.SetMultiplier(0);
        surfCharacter.SetVelocity(Vector3.zero);

        var spawnPositions = Multiplayer.AvatarSpawnLocations;
        gameObject.transform.position = spawnPositions[Random.Range(0, spawnPositions.Count)].position;
    }

    private void HidePlayer(bool hide)
    {
        mainCamera.enabled = !hide;
        GameManager.Instance.deathCamera.enabled = hide;

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
            //GameManager.Instance.hud.SetStocks(Stocks);
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
