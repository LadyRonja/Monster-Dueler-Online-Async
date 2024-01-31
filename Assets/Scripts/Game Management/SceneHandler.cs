using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    public static void LoadGame(string gameID)
    {
        GameLoader.gameIDToLoad = gameID;

        LoadSceneMode("gameScene");
    }

    public static void LoadSceneMode(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
