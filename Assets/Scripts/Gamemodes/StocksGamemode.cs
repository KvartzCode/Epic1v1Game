using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class StocksGamemode : GameMode
{
    bool[] playerDead = new bool[8];
    bool gameIsOver;

    public override void Initialize()
    {
        GameManager.Instance.hud.SetStocks(4);
        gameIsOver = false;
        for (int i = 0; i < playerDead.Length; i++)
        {
            playerDead[i] = false;
        }

    }

    public override void GameModeStart()
    {
        UpdateAllDeathSates();
        GameManager.Instance.playerController.Respawn();
    }

    public override void GameModeUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void GameOver()
    {
        Debug.Log("Game over :)");
        if (GameManager.Instance.user.Index == Multiplayer.LowestUserIndex)
            Invoke(nameof(CallRestart), 5);
    }

    void CallRestart()
    {
        GameManager.Instance.ResetGamemode();
    }

    public override void PlayerDeath(int id)
    {
        GameManager.Instance.hud.RemoveStock();

        if (GameManager.Instance.hud.GetIsDead())
        {
            InvokeRemoteMethod(nameof(SynchSetPlayerDead), Alteruna.UserId.AllInclusive, id, true);
            GameManager.Instance.playerController.SetIsDead(true);
            GameManager.Instance.playerController.HidePlayer(true);
            GameManager.Instance.RemoveSpec();
            CheckIfAllDead();
        }
        else
        {
            StartCoroutine(RespawnLogic());
        }
    }

    private IEnumerator RespawnLogic()
    {
        GameManager.Instance.UpdateAllAvailableSpecPos();
        GameManager.Instance.playerController.SetIsDead(true);
        GameManager.Instance.playerController.HidePlayer(true);
        GameManager.Instance.RemoveSpec();

        yield return new WaitForSeconds(5);
        GameManager.Instance.playerController.Respawn();
        GameManager.Instance.AddSpec();
    }

    private void UpdateAllDeathSates()
    {
        InvokeRemoteMethod(nameof(SynchOwnDeathState), Alteruna.UserId.AllInclusive);
    }

    private void CheckIfAllDead()
    {
        List<User> users = Multiplayer.Instance.GetUsers();
        User aliveUser = null;

        foreach (var item in users)
        {
            if (!playerDead[item.Index])
            {
                if (aliveUser != null)
                    return;

                aliveUser = item;
            }
        }

        if (aliveUser == null)
        {
            Debug.LogError("All dead?");
            return;
        }
        else
        {
            if (!gameIsOver)
                InvokeRemoteMethod(nameof(SynchStartGameOver), UserId.AllInclusive, aliveUser.Index);
        }

    }

    [SynchronizableMethod]
    private void SynchStartGameOver(ulong winnerId)
    {
        gameIsOver = true;
        Debug.Log("Winner is user: " + winnerId);
        GameOver();
    }

    [SynchronizableMethod]
    private void SynchOwnDeathState()
    {
        InvokeRemoteMethod(nameof(SynchSetPlayerDead), Alteruna.UserId.AllInclusive, GameManager.Instance.user.Index, playerDead[GameManager.Instance.user.Index]);
    }

    [SynchronizableMethod]
    private void SynchSetPlayerDead(int id, bool value)
    {
        if (id >= 0 && id < playerDead.Length)
        {
            playerDead[id] = value;
        }
    }

    public override void GameModeJoin()
    {
        UpdateAllDeathSates();
    }
}
