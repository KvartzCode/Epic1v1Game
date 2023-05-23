using Alteruna;
using UnityEngine;

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
    public PlayerHud hud;
    public AudioManager audioManager;

    public Camera deathCamera;

    bool Initialized;

    private void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(gameObject);
        else
            _instance = this;
    }

    private void Start()
    {
        Multiplayer.Instance.RoomLeft.AddListener(EnableMouse);
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
        if(Multiplayer.Instance.InRoom)
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


    public void SetTimeScale(float timeScale, float activeTime, bool isLocal)
    {
        if (!isLocal)
        {
            InvokeRemoteMethod(nameof(SetAllTimeScale), UserId.AllInclusive,timeScale,activeTime);
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


    private new void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(EnableMouse);
        base.OnDestroy();
    }
}
