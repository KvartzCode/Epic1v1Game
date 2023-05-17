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

    float multiplier;

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

    public void UpdateMultiplier()
    {
        InvokeRemoteMethod(nameof(SynchedUpdateMultiplier), UserId.AllInclusive, player.GetMultiplier(), user.Index);
    }

    public void UpdateIdHolder()
    {
        InvokeRemoteMethod(nameof(SynchedUpdateIdHolder), UserId.AllInclusive, user.Index);
    }

    public void UpdateAllMultipliers()
    {
        InvokeRemoteMethod(nameof(SynchedUpdateAllMultipliers), UserId.AllInclusive);
    }

    [SynchronizableMethod]
    public void SynchedUpdateAllMultipliers()
    {
        UpdateMultiplier();
    }

    [SynchronizableMethod]
    void SynchedUpdateIdHolder(int id)
    {
        UserIdHolder idHolder = GameObject.FindWithTag("Player").GetComponent<UserIdHolder>();
        idHolder.SetUserId(user.Index);
    }

    [SynchronizableMethod]
    void SynchedUpdateMultiplier(float multiplier, int id)
    {
        UserIdHolder idHolder = GameObject.FindWithTag("Player").GetComponent<UserIdHolder>();
        idHolder.SetMultiplier(multiplier, id);
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


    private void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(EnableMouse);
    }
}
