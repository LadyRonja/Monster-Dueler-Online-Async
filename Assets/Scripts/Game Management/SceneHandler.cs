using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    public static void LoadGame(string gameID)
    {
        GameLoader.gameIDToLoad = gameID;

        LoadSceneMode("GameScene");
    }

    public static void LoadSceneMode(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void LoadRPSGame(string gameID)
    {
        RPSLoader.rpsGameToLoad = gameID;
        LoadSceneMode("RPSGameScene");
    }
}
