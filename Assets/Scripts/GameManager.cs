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

    public Camera deathCamera;

    public GameModeType currentGamemodeType;

    GameMode currentGamemode;

    [SerializeField] Gamemodes gamemodes;

    bool Initialized;
    bool GamemodeStarted;
    bool createdGame;

    //Specstuffs
    GameObject[] specPos = new GameObject[8];
    List<int> AvailableSpecPos = new List<int>();
    GameObject deathcamPos;
    int currentSpecIndex;

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
        SetUpDeathCameraPos();
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


    public void CreatedGame()
    {
        if (currentGamemode == null)
        {
            Debug.Log("No gamemode selected");
            return;
        }
        currentGamemode.Initialize();
        createdGame = true;

    }

    public void JoinedGame()
    {
        AvailableSpecPos.Add(user.Index);
        if (currentGamemode == null)
        {
            Debug.Log("No gamemode selected");
            return;
        }


        if (!createdGame)
        {
            currentGamemode.Initialize();
            Debug.LogWarning(Multiplayer.GetUsers().Count);
            if (Multiplayer.GetUsers().Count + 1 >= currentGamemode.minimumPlayers)
            {
                Debug.LogWarning("IS HERE0");
                StartCoroutine(WaitForInRoom());
            }
            UpdateAllAvailableSpecPos();
        }
    }

    IEnumerator WaitForInRoom()
    {
        while (!Multiplayer.InRoom)
        {
            yield return null;
            Debug.Log("Here");
        }
        yield return new WaitForSeconds(1);
        InvokeRemoteMethod(nameof(SynchedCheckIfGamemodeStarted), Multiplayer.LowestUserIndex, user.Index);

    }

    public void StartGame()
    {
        GamemodeStarted = true;
        currentGamemode.GameModeStart();
        playerController.Respawn();
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


    private IEnumerator RespawnLogic()
    {
        UpdateAllAvailableSpecPos();
        playerController.SetIsDead(true);
        playerController.HidePlayer(true);
        RemoveSpec();

        yield return new WaitForSeconds(5);

        playerController.SetIsDead(false);
        playerController.HidePlayer(false);
        playerController.Respawn();
        AddSpec();
    }

    private new void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(EnableMouse);
        base.OnDestroy();
    }
}
