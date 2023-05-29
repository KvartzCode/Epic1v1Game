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
    [SerializeField] PlayerAiming playerAiming;
    [SerializeField] ExplosionShootTest shootController;
    [SerializeField] GameObject specCamHolder;
    [SerializeField] float yHardKillFloor = -100f;
    GameObject lobbyCanvas;
    GameObject customizeMenu;

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
            GameManager.Instance.SetSpecObj(System.Convert.ToInt32(avatar.Possessor.Index), specCamHolder);
            return;
        }

        transform.gameObject.layer = LayerMask.NameToLayer("LocalPlayer");
        int children = transform.childCount;
        for (int i = 0; i < children; ++i)
            transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("LocalPlayer");

        GameManager.Instance.SetUser(avatar.Possessor);
        GameManager.Instance.playerController = this;
        Init();
        LockMouse(true);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F1))
            LockMouse(Cursor.visible);

        if (isDead)
        {
            if (Input.GetMouseButtonDown(0))
                GameManager.Instance.MoveSpecCam(true);
            else if (Input.GetMouseButtonDown(1))
                GameManager.Instance.MoveSpecCam(false);

        }
        else
        {
            if (gameObject.transform.position.y < yHardKillFloor)
            {
                isDead = true;
                KillPlayer();
            }
        }


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
        customizeMenu = FindObjectOfType<CustomizeMenu>().gameObject;
        customizeMenu.SetActive(false);

        //hud = Instantiate(PlayerHudToSpawn).GetComponent<PlayerHud>();
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public void RevertCamera(Multiplayer multiplayer)
    {
        mainCamera.transform.parent = null;
        mainCamera.transform.localPosition = originalCameraStats.position;
        mainCamera.transform.rotation = originalCameraStats.rotation;
    }

    public void SetIsDead(bool value)
    {
        isDead = value;
    }

    private void LockMouse(bool isLocked)
    {
        lobbyCanvas.SetActive(!isLocked);
        customizeMenu.SetActive(!isLocked);
        shootController.SetCanShoot(isLocked);
        playerAiming.SetCanAim(isLocked);
        Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isLocked;
    }

    public void Respawn(int spawnIndex)
    {
        GameManager.Instance.audioManager.PlayLocalDeath();
        //TODO: Disable player velocity
        surfCharacter.SetMultiplier(0);
        surfCharacter.SetVelocity(Vector3.zero);
        SetIsDead(false);
        HidePlayer(false);
        var spawnPositions = Multiplayer.AvatarSpawnLocations;
        gameObject.transform.position = spawnPositions[spawnIndex].position;
    }

    public void HidePlayer(bool hide)
    {
        mainCamera.enabled = !hide;
        GameManager.Instance.deathCamera.enabled = hide;
        shootController.SetCanShoot(!hide);
        playerAiming.SetCanAim(!hide);

        //foreach (var c in componentsToHide)
        //{
        //    //c.enabled = !hide;
        //}
    }

    public void KillPlayer()
    {
        GameManager.Instance.audioManager.PlayKOSound(2f, 1000000, transform.position);
        GameManager.Instance.PlayerDeath(GameManager.Instance.user.Index);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone") && !isDead)
        {
            if (avatar.IsMe)
            {
                KillPlayer();
            }
        }
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
