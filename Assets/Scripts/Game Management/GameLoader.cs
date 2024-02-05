using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    public static string gameIDToLoad = "";

    // Development purpose
    string debugGame = "fe0779a1-e95f-4524-971b-ee062444bf97";
    bool usingDebug = true;

    public GameDataRepresentor playerRepresentor;
    public GameDataRepresentor opponentRepresentor;

    private void Awake()
    {
        Debug.Log("should load: " + gameIDToLoad);
        Debug.Log("devgame id: " + debugGame);
        Invoke(nameof(FetchGame), 1);
    }

    private async void FetchGame()
    {
        if (gameIDToLoad == "" && !usingDebug)
        {
            Debug.LogError("No game to load");
            return;
        }
        /*
        // Development purpse
        if (usingDebug)
            await Login.AttemptLogin("test0@test.test", "pass123");*/

        string gameBlob;
        if (usingDebug)
            gameBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.GAMES_TABLE, debugGame);
        else
            gameBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.GAMES_TABLE, gameIDToLoad);
        

        if (gameBlob == null || gameBlob == "") {

            Debug.LogError("Game not found");
            return;
        }


        Game gameToLoad = JsonUtility.FromJson<Game>(gameBlob);

        // TODO:
        // Validate game state here

        LoadGame(gameToLoad);
    }

    private void LoadGame(Game gameToLoad)
    {
        Debug.Log($"Signed in player: {ActiveUser.CurrentActiveUser.username}");

       if(usingDebug)
            Debug.Log("Loading game with current information: " + debugGame);
        else
            Debug.Log("Loading game with current information: " + gameIDToLoad);


        Debug.Log($"Player A: {gameToLoad.players[0].username}");
        Debug.Log($"Player B: {gameToLoad.players[1].username}");

        Debug.Log($"Player A monster healths: " +
            $"{gameToLoad.gameDatas[0].monsters[0].curHealth} / {gameToLoad.gameDatas[0].monsters[0].maxHealth}, " +
            $"{gameToLoad.gameDatas[0].monsters[1].curHealth} / {gameToLoad.gameDatas[0].monsters[1].maxHealth}, " +
            $"{gameToLoad.gameDatas[0].monsters[2].curHealth} / {gameToLoad.gameDatas[0].monsters[2].maxHealth} ");

        Debug.Log($"Player B monster healths: " +
            $"{gameToLoad.gameDatas[1].monsters[0].curHealth} / {gameToLoad.gameDatas[1].monsters[0].maxHealth}, " +
            $"{gameToLoad.gameDatas[1].monsters[1].curHealth} / {gameToLoad.gameDatas[1].monsters[1].maxHealth}, " +
            $"{gameToLoad.gameDatas[1].monsters[2].curHealth} / {gameToLoad.gameDatas[1].monsters[2].maxHealth} ");
    }
}
