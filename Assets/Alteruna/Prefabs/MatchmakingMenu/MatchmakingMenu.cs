using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Alteruna.Trinity;
using Alteruna;

public class MatchmakingMenu : MonoBehaviour
{
    public enum GameState
    {
        Started,
        Searching,
        Filling,
        Full,
        Playing,
    }

    [SerializeField]
    private Multiplayer AUMP;

    public GameState State;
    public GameObject ConnectedIndicator;
    public Text ConnectedPlayersText;
    public GameObject MatchmakingButton;
    public GameObject ForceStartButton;
    public GameObject StartButton;
    public GameObject ReadyButton;

    public GameObject SearchingText;
    public GameObject WaitingText;
    public GameObject ConnectingText;
    public GameObject PlayersInfoText;

    public GameObject Content;
    public GameObject EntryPrefab;

    public UnityEvent<Multiplayer, int> GameStarted;

    [HideInInspector]
    public List<MatchmakingEntry> PlayerEntries = new List<MatchmakingEntry>();
    [HideInInspector]
    public bool IsOwner;
    [HideInInspector]
    public bool IsReady;
    [HideInInspector]
    public int ConnectedUsers = 0;
    private Room _room;

    public void JoinMatchmaking()
    {
        if (AUMP.IsConnected)
        {
            AUMP?.JoinMatchmaking();
            State = GameState.Searching;
            SearchingText.gameObject.SetActive(true);
            MatchmakingButton.SetActive(false);
        }
    }

    public void StartIfAllReady()
    {
        if (!IsOwner)
        {
            return;
        }

        int len = PlayerEntries.Count;
        for (int i = 0; i < len; i++)
        {
            if (!PlayerEntries[i].Ready)
            {
                return;
            }
        }

        if (ConnectedUsers >= AUMP.CurrentRoom.MaxUsers)
        {
            StartGame();
        }
    }

    public void ForceStart()
    {
        if (!IsOwner)
        {
            return;
        }

        if (IsOwner)
        {
            // TODO: Set updateState on room
            //_room?.UpdateState(SessionState.Busy);
            AUMP.LockRoom();
        }

        StartGame();
    }

    public void StartGame()
    {
        if (IsOwner)
        {
            AUMP.InvokeRemoteProcedure("StartGame", (ushort)UserId.All);
            AUMP.LockRoom();
        }

        GameStarted.Invoke(AUMP, ConnectedUsers);
        State = GameState.Playing;
        gameObject.SetActive(false);
    }

    private void RemoteStartedGame(ushort fromUser, ProcedureParameters p, uint callID, ITransportStreamReader reader)
    {
        StartGame();
    }

    private void RemoteMarkedReady(ushort fromUser, ProcedureParameters p, uint callID, ITransportStreamReader reader)
    {
        int len = PlayerEntries.Count;
        for (int i = 0; i < len; i++)
        {
            if (PlayerEntries[i].Id == fromUser)
            {
                bool ready;
                p.Get("isReady", out ready);
                PlayerEntries[i].SetReady(ready);
            }
        }
    }

    public void ToggleReady()
    {
        IsReady = !IsReady;
        PlayerEntries[0].SetReady(IsReady);
        ProcedureParameters parameters = new ProcedureParameters();
        parameters.Set("isReady", IsReady);
        AUMP?.InvokeRemoteProcedure("MarkReady", (ushort)UserId.All, parameters);
    }

    public void Start()
    {
        State = GameState.Started;
        ResetMenu();

        if (AUMP == null)
        {
            AUMP = FindObjectOfType<Multiplayer>();
        }

        if (AUMP != null)
        {
            AUMP.Disconnected.AddListener(OnDisconnected);
            AUMP.Connected.AddListener(OnConnected);
            AUMP.RoomJoined.AddListener(OnJoined);
            AUMP.RoomLeft.AddListener(OnLeft);
            AUMP.OtherUserJoined.AddListener(OnOtherJoined);
            AUMP.OtherUserLeft.AddListener(OnOtherLeft);

            AUMP.RegisterRemoteProcedure("MarkReady", RemoteMarkedReady);
            AUMP.RegisterRemoteProcedure("StartGame", RemoteStartedGame);
        }
    }

    public void ResetMenu()
    {
        State = GameState.Started;
        StartButton.SetActive(false);
        ForceStartButton.SetActive(false);
        ReadyButton.SetActive(false);
        SearchingText.SetActive(false);
        WaitingText.SetActive(false);
        ConnectingText.SetActive(true);
        PlayersInfoText.SetActive(false);
        
        int len = PlayerEntries.Count;
        for (int i = 0; i < len; i++)
        {
            Destroy(PlayerEntries[i].gameObject);
        }

        PlayerEntries.Clear();
    }

    public void OnConnected(Multiplayer multiplayer, Endpoint endpoint)
    {
        ConnectedIndicator.SetActive(true);
        ConnectingText.SetActive(false);
        MatchmakingButton.SetActive(true);
    }

    public void OnDisconnected(Multiplayer multiplayer, Endpoint endpoint)
    {
        ConnectedIndicator.SetActive(false);
        _room = null;

        ConnectedUsers = 0;
        UpdateConnectedPlayersText();

        ResetMenu();
    }

    public void OnJoined(Multiplayer multiplayer, Room room, User user)
    {
        State = GameState.Filling;
        _room = room;

        if (user.Index == 0)
        {
            IsOwner = true;
            StartButton.SetActive(true);
            ForceStartButton.SetActive(true);
        }
        else
        {
            WaitingText.SetActive(true);
        }

        ReadyButton.SetActive(true);
        SearchingText.SetActive(false);
        PlayersInfoText.SetActive(true);

        ConnectedUsers++;
        UpdateConnectedPlayersText();
        AddPlayerEntry(multiplayer.Me.Name, user.Index);
    }

    public void OnLeft(Multiplayer multiplayer)
    {
        _room = null;
        ConnectedUsers = 0;
        UpdateConnectedPlayersText();
    }

    public void OnOtherJoined(Multiplayer multiplayer, User user)
    {
        ConnectedUsers++;
        UpdateConnectedPlayersText();

        AddPlayerEntry(user.Name, user.Index);
    }

    public void OnOtherLeft(Multiplayer multiplayer, User user)
    {
        ConnectedUsers--;
        UpdateConnectedPlayersText();

        RemovePlayerEntry(user.Index);
    }

    void UpdateConnectedPlayersText()
    {
        ConnectedPlayersText.text = "(" + ConnectedUsers + "/" + AUMP.CurrentRoom.MaxUsers + ")";
    }

    void RemovePlayerEntry(ushort id)
    {
        int len = PlayerEntries.Count;
        for (int i = 0; i < len; i++)
        {
            if (PlayerEntries[i].Id == id)
            {
                GameObject entry = PlayerEntries[i].gameObject;
                PlayerEntries.Remove(PlayerEntries[i]);
                Destroy(entry);
            }
        }
    }

    void AddPlayerEntry(string name, ushort id)
    {
        GameObject entry = Instantiate(EntryPrefab, Content.transform);
        entry.SetActive(true);
        MatchmakingEntry player = entry.GetComponentInChildren<MatchmakingEntry>();
        player.NameText.text = name;
        player.Id = id;
        player.SetReady(false);

        if (id == 0)
        {
            player.SetOwner(true);
        }
        else
        {
            player.SetOwner(false);
        }

        PlayerEntries.Add(player);

        if (ConnectedUsers >= AUMP.CurrentRoom.MaxUsers)
        {
            State = GameState.Full;
        }
    }

    void Reset()
    {
        if (AUMP == null)
        {
            AUMP = FindObjectOfType<Multiplayer>();
        }
    }
}