using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Alteruna;

public abstract class GameMode : AttributesSync
{
    public int minimumPlayers;

    /// <summary>
    /// Used for resetting the gamemode
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Start function for gamemode
    /// </summary>
    public abstract void GameModeStart();
    /// <summary>
    /// Start function for when joining late
    /// </summary>
    public abstract void GameModeJoin();
    /// <summary>
    /// Uptade function for gamemode
    /// </summary>
    public abstract void GameModeUpdate();
    public abstract void GameOver();
    public abstract void PlayerDeath(int id);

}
