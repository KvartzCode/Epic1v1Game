using Alteruna;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameModeType { Sandbox, Stocks }

[System.Serializable]
public class Gamemodes
{
    public StocksGamemode stocks;
}

public class GameManager : AttributesSync
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public User user;
    public Fragsurf.Movement.SurfCharacter player;
    public PlayerController playerController;
    public PlayerHud hud;
    public AudioManager audioManager;

    public GameObject ServerCreatorMenu;

    public Level[] levels;
    public int currentLevelInt = 0;
    [SerializeField] GameObject spawnedLevel;

    public Camera deathCamera;

    public GameModeType currentGamemodeType;

    LevelData currentLevelData;



    GameMode currentGamemode;

    [SerializeField] Gamemodes gamemodes;

    bool Initialized;
    bool GamemodeStarted;
    bool createdGame;

    bool hasGottenGamemode;
    float getGamemodeTimeOutTimer;

    //Specstuffs
    GameObject[] specPos = new GameObject[8];
    List<int> AvailableSpecPos = new List<int>();
    GameObject deathcamPos;
    int currentSpecIndex;

    int levelToSelect;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;


        //currentGamemode = gamemodes.stocks;
    }

    private void Start()
    {
        Multiplayer.Instance.RoomLeft.AddListener(EnableMouse);
    }

    private void Update()
    {


        if (Multiplayer.InRoom)
        {
            if (playerController != null)
            {
                if (playerController.GetIsDead())
                {
                    if (specPos[AvailableSpecPos[currentSpecIndex]] != null)
                    {
                        deathCamera.transform.position = specPos[AvailableSpecPos[currentSpecIndex]].transform.position;
                        deathCamera.transform.rotation = specPos[AvailableSpecPos[currentSpecIndex]].transform.rotation;
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.F7))
                currentGamemodeType = GameModeType.Stocks;
        }
    }

    private void EnableMouse(Multiplayer m)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    #region Sync Methods

    public void SynchedUpdateMultiplier()
    {
        InvokeRemoteMethod(nameof(UpdateMultiplier), UserId.AllInclusive, player.GetMultiplier(), user.Index);
    }

    [SynchronizableMethod]
    void UpdateMultiplier(float multiplier, int id)
    {
        UserIdHolder idHolder = GameObject.FindWithTag("Player").GetComponent<UserIdHolder>();
        idHolder.SetMultiplier(multiplier, id);
    }


    public void SynchedUpdateIdHolder()
    {
        InvokeRemoteMethod(nameof(UpdateIdHolder), UserId.AllInclusive, user.Index);
    }

    [SynchronizableMethod]
    void UpdateIdHolder(int id)
    {
        UserIdHolder idHolder = GameObject.FindWithTag("Player").GetComponent<UserIdHolder>();
        idHolder.SetUserId(user.Index);
    }


    public void SynchedUpdateHats()
    {
        if (Multiplayer.Instance.InRoom)
        {
            InvokeRemoteMethod(nameof(UpdateHats), UserId.AllInclusive);
        }
    }

    [SynchronizableMethod]
    void UpdateHats()
    {
        UserIdHolder idHolder = GameObject.FindWithTag("Player").GetComponent<UserIdHolder>();
        idHolder.SetHat(CosmeticManager.Instance.GetCurrentHat());
    }


    public void SynchedUpdateAllMultipliers()
    {
        InvokeRemoteMethod(nameof(UpdateAllMultipliers), UserId.AllInclusive);
    }

    [SynchronizableMethod]
    void UpdateAllMultipliers()
    {
        SynchedUpdateMultiplier();
    }

    #endregion

    #region Spectate Stuff

    public void SetSpecObj(int index, GameObject pos)
    {
        if (index >= 0 && 0 < specPos.Length)
            specPos[index] = pos;
        else
            Debug.LogError("INVALID INDEX");
    }

    void SetUpDeathCameraPos()
    {
        if (deathcamPos == null)
        {
            deathcamPos = new GameObject("DeathCameraPos");
            deathcamPos.transform.position = deathCamera.transform.position;
            deathcamPos.transform.rotation = deathCamera.transform.rotation;
        }
        else if (AvailableSpecPos.Count > 0)
        {
            specPos[AvailableSpecPos[0]] = null;
        }

        AvailableSpecPos = new List<int>();
        AvailableSpecPos.Add(user.Index);
        specPos[user.Index] = deathcamPos;
    }

    public void UpdateAllAvailableSpecPos()
    {
        InvokeRemoteMethod(nameof(SynchUpdateAllAvailableSpecPos), UserId.AllInclusive);
    }

    [SynchronizableMethod]
    void SynchUpdateAllAvailableSpecPos()
    {
        if (playerController.GetIsDead())
            RemoveSpec();
        else
            AddSpec();
    }

    public void AddSpec()
    {
        InvokeRemoteMethod(nameof(SynchAddSpec), UserId.All, user.Index);
    }


    public void RemoveSpec()
    {
        InvokeRemoteMethod(nameof(SynchRemoveSpec), UserId.All, user.Index);
    }

    [SynchronizableMethod]
    void SynchAddSpec(int id)
    {
        AvailableSpecPos.Add(id);
    }

    [SynchronizableMethod]
    void SynchRemoveSpec(int id)
    {
        if (AvailableSpecPos[currentSpecIndex] == id)
            ForceMoveSpecCam(0);

        AvailableSpecPos.Remove(id);
    }

    public void MoveSpecCam(bool moveForward)
    {
        if (moveForward)
            currentSpecIndex = (currentSpecIndex + 1) >= AvailableSpecPos.Count ? 0 : currentSpecIndex + 1;
        else
            currentSpecIndex = (currentSpecIndex - 1) < 0 ? AvailableSpecPos.Count - 1 : 0;

    }

    void ForceMoveSpecCam(int Index)
    {
        deathCamera.transform.position = specPos[AvailableSpecPos[0]].transform.position;
        deathCamera.transform.rotation = specPos[AvailableSpecPos[0]].transform.rotation;
    }

    #endregion
    public void PlayerDeath(int id)
    {
        if (GamemodeStarted)
        {
            currentGamemode.PlayerDeath(id);
        }
        else
        {
            StartCoroutine(RespawnLogic());
        }
    }

    public void SetTimeScale(float timeScale, float activeTime, bool isLocal)
    {
        if (!isLocal)
        {
            InvokeRemoteMethod(nameof(SetAllTimeScale), UserId.AllInclusive, timeScale, activeTime);
            return;
        }
        Time.timeScale = timeScale;
        Invoke(nameof(ResetTimeScale), activeTime);
    }

    [SynchronizableMethod]
    void SetAllTimeScale(float timeScale, float activeTime)
    {
        Time.timeScale = timeScale;
        Invoke(nameof(ResetTimeScale), activeTime);
    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

    public void SetUser(User user)
    {
        this.user = user;
    }


    #region Host Logic

    public static void LeaveRoom()
    {
        if (Multiplayer.Instance.Me.Index != Multiplayer.Instance.GetHost())
            Multiplayer.Instance.CurrentRoom?.Leave();
        else
            Multiplayer.Instance.CurrentRoom?.Destroy();
    }

    #endregion


    public void GetGamemode()
    {
        if (user.Index == Multiplayer.LowestUserIndex)
        {
            InvokeRemoteMethod(nameof(SynchedGetGamemode), FindSeccondLowestUser(), user.Index);
        }
        else
        {
            InvokeRemoteMethod(nameof(SynchedGetGamemode), Multiplayer.LowestUserIndex, user.Index);
        }
    }

    [SynchronizableMethod]
    void SynchedSetGamemode(GameModeType type)
    {
        Debug.Log("type is: " + type);
        currentGamemodeType = type;
        SelectGamemode();
    }

    [SynchronizableMethod]
    void SynchedGetGamemode(ushort id)
    {
        Debug.Log("I WILL RETURN" + currentGamemodeType);
        InvokeRemoteMethod(nameof(SynchedSetGamemode), id, currentGamemodeType);
    }

    public void GetLevel()
    {
        InvokeRemoteMethod(nameof(SynchedGetLevel), Multiplayer.Instance.LowestUserIndex, user.Index);
    }

    [SynchronizableMethod]
    void SynchedGetLevel(ushort userId)
    {
        Debug.LogWarning("IS HERE");
        InvokeRemoteMethod(nameof(SynchedSetLevel), userId, currentLevelInt);
    }

    [SynchronizableMethod]
    void SynchedSetLevel(int level)
    {
        SelectLevel(level);
    }

    public void CreatedGame()
    {
        createdGame = true;
        StartCoroutine(CreateGameWaitForInRoom());
    }

    IEnumerator CreateGameWaitForInRoom()
    {
        while (!Multiplayer.Instance.InRoom)
        {
            yield return null;
        }

        SelectGamemode();
        SelectLevel(levelToSelect);
        hasGottenGamemode = false;

        if (currentGamemode != null)
            currentGamemode.Initialize();

        yield return new WaitUntil(() => playerController != null);
        playerController.Respawn();
    }

    void SelectLevel(int level)
    {

        if (spawnedLevel != null)
        {
            if (spawnedLevel != levels[level])
            {
                Destroy(spawnedLevel);
                spawnedLevel = Instantiate(levels[level].Prefab, Vector3.zero, levels[level].Prefab.transform.rotation);
            }
        }
        currentLevelInt = level;
        currentLevelData = spawnedLevel.GetComponent<LevelData>();
        SetLevelData();
    }

    void SelectGamemode()
    {
        switch (currentGamemodeType)
        {
            case GameModeType.Sandbox:
                currentGamemode = null;
                break;
            case GameModeType.Stocks:
                currentGamemode = gamemodes.stocks;
                break;
            default:
                break;
        }

        hasGottenGamemode = true;
    }

    void SetLevelData()
    {
        deathcamPos = currentLevelData.deathCamPos;
        SetUpDeathCameraPos();
        Multiplayer.Instance.AvatarSpawnLocations = currentLevelData.spawnPoints;
    }

    public void JoinedGame()
    {
        StartCoroutine(JoinGame());
    }

    IEnumerator JoinGame()
    {
        ServerCreatorMenu.SetActive(false);
        StartCoroutine(WaitForInRoomSpecPos());
        if (!createdGame)
        {
            while (!Multiplayer.Instance.InRoom)
            {
                yield return null;
            }

            yield return null;
            GetGamemode();
            while (!hasGottenGamemode)
            {
                getGamemodeTimeOutTimer += Time.deltaTime;
                yield return null;
                if (getGamemodeTimeOutTimer > 10)
                {
                    Debug.LogWarning("TIMED OUT");
                    break;
                }
            }
            GetLevel();

            getGamemodeTimeOutTimer = 0;
            if (!hasGottenGamemode)
            {
                Multiplayer.CurrentRoom.Leave();
                Debug.LogError("USER TIMED OUT");
            }
        }


        if (hasGottenGamemode)
        {

            if (currentGamemode != null)
            {

                if (!createdGame)
                {
                    Debug.LogWarning(Multiplayer.GetUsers().Count);
                    if (Multiplayer.GetUsers().Count + 1 >= currentGamemode.minimumPlayers)
                    {
                        StartCoroutine(WaitForInRoomGamemode());
                    }
                    UpdateAllAvailableSpecPos();
                }
            }
        }

        hasGottenGamemode = false;
    }

    public void LevelToSelect(int level)
    {
        levelToSelect = level;
    }

    IEnumerator WaitForInRoomSpecPos()
    {
        while (!Multiplayer.InRoom)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        AvailableSpecPos.Add(user.Index);
    }
    IEnumerator WaitForInRoomGamemode()
    {
        while (!Multiplayer.InRoom)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        InvokeRemoteMethod(nameof(SynchedCheckIfGamemodeStarted), Multiplayer.LowestUserIndex, user.Index);

    }

    public void StartGame()
    {
        GamemodeStarted = true;
        currentGamemode.GameModeStart();
    }

    public void SetStateCreateMenu(bool state)
    {
        if (!Multiplayer.InRoom)
            ServerCreatorMenu.SetActive(state);
        else if (ServerCreatorMenu.activeSelf)
            ServerCreatorMenu.SetActive(false);

    }

    User FindSeccondLowestUser()
    {
        List<User> users = Multiplayer.GetUsers();
        User lowestUser = user;
        for (int i = 0; i < users.Count; i++)
        {
            if (users[i] != user)
            {
                if (lowestUser == user)
                    lowestUser = users[i];
                else if (lowestUser.Index > users[i])
                    lowestUser = users[i];
            }
        }
        return lowestUser;
    }


    [SynchronizableMethod]
    void SynchedSetGamemodeStarted(bool value)
    {
        GamemodeStarted = value;
        if (GamemodeStarted == true)
            currentGamemode.GameModeJoin();
    }

    [SynchronizableMethod]
    void SynchedStartGame()
    {
        StartGame();
    }


    [SynchronizableMethod]
    void SynchedCheckIfGamemodeStarted(ushort id)
    {
        if (GamemodeStarted)
            InvokeRemoteMethod(nameof(SynchedSetGamemodeStarted), id, GamemodeStarted);
        else
            InvokeRemoteMethod(nameof(SynchedStartGame), UserId.AllInclusive);
    }
    public void ResetGamemode()
    {
        InvokeRemoteMethod(nameof(SynchResetGamemode), UserId.AllInclusive);
    }

    [SynchronizableMethod]
    void SynchResetGamemode()
    {
        currentGamemode.Initialize();
        if (user.Index == Multiplayer.LowestUserIndex)
            InvokeRemoteMethod(nameof(SynchedStartGame), UserId.AllInclusive);
    }

    private IEnumerator RespawnLogic()
    {
        UpdateAllAvailableSpecPos();
        playerController.SetIsDead(true);
        playerController.HidePlayer(true);
        RemoveSpec();

        yield return new WaitForSeconds(5);
        playerController.Respawn();
        AddSpec();
    }

    public void AddForceOnPlayer(ushort playerId, Vector3 dir, float force, bool useMultiplier, bool checkKo = false, float checkKoMultiplier = 1f)
    {
        InvokeRemoteMethod(nameof(SynchedAddForceOnPlayer), playerId, dir, force, useMultiplier, checkKo, checkKoMultiplier);
    }

    [SynchronizableMethod]
    public void SynchedAddForceOnPlayer(Vector3 dir, float force, bool useMultiplier, bool checkKo = false, float checkKoMultiplier = 1f)
    {
        if (checkKo)
            if (CheckKO(dir, player.gameObject, force * checkKoMultiplier, true))
                force *= 3;

        player.AddVelocity(dir, force, useMultiplier);
    }

    public bool CheckKO(Vector3 direction, GameObject other, float force, bool applyForce = true)
    {
        // Debug.Log("Entered CheckKO method.");

        float threshhold = 40;
        Vector3 pos = other.transform.position;
        float multiplier = applyForce ? other.GetComponent<Fragsurf.Movement.SurfCharacter>().GetMultiplier() : (other.GetComponent<UserIdHolder>().GetMultiplier() * 0.01f);

        //Debug.Log("Force multiplier: " + (force * multiplier).ToString());

        if (force * multiplier < threshhold)
        {
            // Debug.Log("Force times multiplier is less than threshold. Returning false.");
            return false;
        }
        else
        {
            int layerMask = 1 << 3;
            layerMask = ~layerMask; // invert to ignore layer 3

            RaycastHit hit;
            if (Physics.Raycast(pos, direction, out hit, Mathf.Infinity, layerMask))
            {
                //Debug.Log("Raycast hit an object: " + hit.collider.gameObject.name);
                // Check if the first object the raycast hit is a trigger and has the tag "DeathZone"
                if (hit.collider.isTrigger && hit.collider.tag == "DeathZone")
                {
                    if (applyForce)
                    {
                        // Debug.Log("Hit object is a trigger and tagged as 'DeathZone'. Returning true.");
                        other.GetComponent<Fragsurf.Movement.SurfCharacter>().SetVelocity(Vector3.zero);
                        GameManager.Instance.audioManager.PlayGlobal3DSoundEffect(0, 1.5f, 10000, transform.position);
                        GameManager.Instance.audioManager.PlayLocal2DSoundEffect(0, 1f, 100);
                        GameManager.Instance.SetTimeScale(0.01f, 0.01f, true);
                    }
                    return true;
                }
                else
                {
                    Debug.Log("Hit object is either not a trigger or not tagged as 'DeathZone'. Returning false.");
                }
            }
            else
            {
                Debug.Log("No raycast hit detected. Returning false.");
            }
        }

        return false;
    }

    private new void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(EnableMouse);
        base.OnDestroy();
    }
}
