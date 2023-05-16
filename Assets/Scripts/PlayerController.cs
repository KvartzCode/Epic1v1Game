using Alteruna;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Alteruna.Avatar))]
public class PlayerController : AttributesSync
{
    private Alteruna.Avatar avatar;

    [SerializeField] Transform cameraHolder;
    private OriginalCameraStats originalCameraStats;

    private int clientInfo1 = 3, clientInfo2 = 5;
    private Spawner spawner;


    void Awake()
    {
        avatar = GetComponent<Alteruna.Avatar>();
        spawner = FindObjectOfType<Spawner>();
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
        if (Input.GetKeyDown(KeyCode.F))
            ClientRequestHostLogic();
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(GameManager.Instance.user.Index);
            Debug.Log("Host ID = " + Multiplayer.Instance.GetUser(Multiplayer.Instance.LowestUserIndex).Index);
        }
        if (Input.GetKeyDown(KeyCode.Q))
            GameManager.LeaveRoom();
            //CloseRoom();
    }


    void Init()
    {
        originalCameraStats = new OriginalCameraStats(Camera.main.transform);
        Camera.main.transform.SetParent(cameraHolder);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.rotation = cameraHolder.rotation;
        Multiplayer.Instance.RoomLeft.AddListener(RevertCamera);
    }

    public void RevertCamera(Multiplayer multiplayer)
    {
        Camera.main.transform.parent = null;
        Camera.main.transform.localPosition = originalCameraStats.position;
        Camera.main.transform.rotation = originalCameraStats.rotation;
    }

    private new void OnDestroy()
    {
        Multiplayer.Instance.RoomLeft.RemoveListener(RevertCamera);
        base.OnDestroy();
    }


    #region Host Logic

    void ClientRequestHostLogic()
    {
        InvokeRemoteMethod(nameof(CallHost), Multiplayer.Instance.GetHost(), clientInfo1, clientInfo2);
    }

    [SynchronizableMethod]
    void CallHost(int userInformation, int moreUserInformation)
    {
        InvokeRemoteMethod(nameof(AllClientsRecieveThis), UserId.AllInclusive, userInformation + moreUserInformation);
    }

    [SynchronizableMethod]
    void AllClientsRecieveThis(int recievedInformation)
    {
        Debug.Log($"All clients recieve this information: {recievedInformation} // This was calculated only on the earliest client");
    }

    #endregion


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
