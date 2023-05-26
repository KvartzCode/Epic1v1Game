using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Alteruna;
using TMPro;

public class ServerCreator : MonoBehaviour
{
    [SerializeField] RawImage levelImage;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] TextMeshProUGUI gamemodeText;
    [SerializeField] string[] defaultNames;

    Level currentLevel;
    int levelIndex = 0;
    string roomname = "";
    GameModeType gamemode = GameModeType.Sandbox;

    GameObject spawnedLevelForComponents;

    private IEnumerator Start()
    {
        yield return null;
        currentLevel = GameManager.Instance.levels[levelIndex].GetComponent<Level>();
        UpdateUi();
    }


    public void SwitchGamemode(bool forward = true)
    {
        var enumCount = Enum.GetNames(typeof(GameModeType)).Length;

        gamemode = forward ? (((int)gamemode + 1) >= enumCount ? (GameModeType)0 : (GameModeType)((int)gamemode + 1)) : ((GameModeType)((int)gamemode - 1) < (GameModeType)0) ? (GameModeType)(enumCount - 1) : (GameModeType)((int)gamemode - 1);
        UpdateUi();
    }


    public void SwitchLevel(bool forward = true)
    {
        int count = GameManager.Instance.levels.Length;

        levelIndex = forward ? ((levelIndex + 1) < count ? (levelIndex + 1) : 0) : ((levelIndex - 1) < 0 ? (count - 1) : (levelIndex - 1));

        if (spawnedLevelForComponents != null)
            Destroy(spawnedLevelForComponents);

        spawnedLevelForComponents = Instantiate(GameManager.Instance.levels[levelIndex], Vector3.one * 1000, Quaternion.identity);
        currentLevel = spawnedLevelForComponents.GetComponent<Level>();
        UpdateUi();
    }

    public void Submit()
    {
        GameManager manager = GameManager.Instance;


        manager.LevelToSelect(levelIndex);
        manager.currentGamemodeType = gamemode;

        if (spawnedLevelForComponents != null)
            Destroy(spawnedLevelForComponents);

        if (roomname == "")
            roomname = defaultNames[UnityEngine.Random.Range(0, defaultNames.Length)];

        Multiplayer.Instance.CreateRoom(roomname);
        manager.CreatedGame();

    }

    public void UpdateName(string name)
    {
        roomname = name;
    }

    void UpdateUi()
    {
        if (currentLevel == null)
        {
            if (spawnedLevelForComponents != null)
                Destroy(spawnedLevelForComponents);

            spawnedLevelForComponents = Instantiate(GameManager.Instance.levels[levelIndex], Vector3.one * 1000, Quaternion.identity);
            currentLevel = spawnedLevelForComponents.GetComponent<Level>();
        }

        levelText.text = currentLevel.levelName;
        levelImage.texture = currentLevel.levelImage;
        gamemodeText.text = gamemode.ToString();
    }
}
