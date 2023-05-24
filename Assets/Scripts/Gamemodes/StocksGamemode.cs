using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public class StocksGamemode : GameMode
{
    bool[] playerDead = new bool[8];

    public override void Initialize()
    {
        for (int i = 0; i < playerDead.Length; i++)
        {
            playerDead[i] = false;
        }
    }

    public override void GameModeStart()
    {
        UpdateAllDeathSates();
    }

    public override void GameModeUpdate()
    {
        throw new System.NotImplementedException();
    }

    public override void GameOver()
    {
        Debug.Log("Game over :)");
    }


    public override void PlayerDeath(int id)
    {
        GameManager.Instance.hud.RemoveStock();

        if (GameManager.Instance.hud.GetIsDead())
        {
            InvokeRemoteMethod(nameof(SynchSetPlayerDead), Alteruna.UserId.AllInclusive, id, true);
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

        GameManager.Instance.playerController.SetIsDead(false);
        GameManager.Instance.playerController.HidePlayer(false);
        GameManager.Instance.playerController.Respawn();
        GameManager.Instance.AddSpec();
    }

    private void UpdateAllDeathSates()
    {
        InvokeRemoteMethod(nameof(SynchOwnDeathState), Alteruna.UserId.AllInclusive);
    }

    private void CheckIfAllDead()
    {
        Debug.LogError("EEEEEEERERERERERERER");
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

        Debug.Log("IS HERE");

        if (aliveUser == null)
        {
            Debug.LogError("All dead?");
            return;
        }
        else
        {
            InvokeRemoteMethod(nameof(SynchStartGameOver), UserId.AllInclusive, aliveUser.Index);
        }

    }

    [SynchronizableMethod]
    private void SynchStartGameOver(ulong winnerId)
    {
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
