using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RPSLoader : MonoBehaviour
{
    // Singleton
    private static RPSLoader instance;
    public static RPSLoader Instance { get => GetInstance(); private set => instance = value; }

    public static string rpsGameToLoad = "";    
    // Development purpose
    [HideInInspector] public string debugGame = "d228ea88-ef7d-4cd4-8597-33add654dddf";
    [HideInInspector] public bool usingDebug = false;

    [SerializeField] GameObject moveEntryPrefab;
    [SerializeField] Transform activePlayerEntryParent;
    [SerializeField] Transform opponentPlayerEntryParent;
    [Space]
    [SerializeField] TMP_Text winsText;
    [SerializeField] TMP_Text tiesText;
    [SerializeField] TMP_Text lossesText;

    [HideInInspector]
    public RPSGame loadedGame;
    public List<RPSMove> activePlayerMoves;
    public List<RPSMove> opponentMoves;

    private void Awake()
    {
        #region Singleton
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
        #endregion

        Debug.Log("should load: " + rpsGameToLoad);
        Debug.Log("devgame id: " + debugGame);
        foreach (Transform child in activePlayerEntryParent)
            Destroy(child.gameObject);
        foreach (Transform child in opponentPlayerEntryParent)
            Destroy(child.gameObject);

        Invoke(nameof(FetchGame), 1);
    }

    private async void FetchGame()
    {
        if (rpsGameToLoad == "" && !usingDebug)
        {
            Debug.LogError("No game to load");
            return;
        }

        string gameBlob;
        if (usingDebug)
        {
            gameBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.RPS_TABLE, debugGame);
            string activeUserBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.USER_TABLE, "9GZbdeUZ5PPlkkmZAJMgJPvKoDj2");
            ActiveUser.SetActiveUser(JsonUtility.FromJson<User>(activeUserBlob));
        }
        else
            gameBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.RPS_TABLE, rpsGameToLoad);


        if (gameBlob == null || gameBlob == "")
        {

            Debug.LogError("Game not found");
            return;
        }


        RPSGame gameToLoad = JsonUtility.FromJson<RPSGame>(gameBlob);

        // TODO:
        // Validate game state here

        LoadGame(gameToLoad);
    }

    private void LoadGame(RPSGame gameToLoad)
    {
        Debug.Log($"Signed in player: {ActiveUser.CurrentActiveUser.username}");

        if (usingDebug)
            Debug.Log("Loading game with current information: " + debugGame);
        else
            Debug.Log("Loading game with current information: " + rpsGameToLoad);

        int movesToLoad = Mathf.Min(gameToLoad.playerAMoves.Count, gameToLoad.playerBMoves.Count);
        
        activePlayerMoves = new();
        opponentMoves = new();

        if (gameToLoad.playerA == ActiveUser.CurrentActiveUser.username)
        {
            activePlayerMoves = gameToLoad.playerAMoves;
            opponentMoves = gameToLoad.playerBMoves;
        }
        else
        {
            activePlayerMoves = gameToLoad.playerBMoves;
            opponentMoves = gameToLoad.playerAMoves;
        }

        int wins = 0;
        int ties = 0;
        int losses = 0;
        for (int i = 0; i < movesToLoad; i++)
        {
            GameObject aMove = Instantiate(moveEntryPrefab, activePlayerEntryParent);
            TMP_Text aText = aMove.GetComponentInChildren<TMP_Text>();
            aText.text = ((RPS)activePlayerMoves[i].selectedMove).ToString(); 

            GameObject oMove = Instantiate(moveEntryPrefab, opponentPlayerEntryParent);
            TMP_Text oText = oMove.GetComponentInChildren<TMP_Text>();
            oText.text = ((RPS)opponentMoves[i].selectedMove).ToString();

            switch (activePlayerMoves[i].selectedMove)
            {
                case RPS.UNSELECTED:
                    Debug.LogError("Please contact IT, this shouldn't be allowed");
                    break;
                case RPS.ROCK:
                    switch (opponentMoves[i].selectedMove)
                    {
                        case RPS.ROCK:
                            ties++;
                            aText.color = Color.yellow;
                            oText.color = Color.yellow;
                            break;
                        case RPS.PAPER:
                            losses++;
                            aText.color = Color.red;
                            oText.color = Color.green;
                            break;
                        case RPS.SCISSOR:
                            wins++;
                            aText.color = Color.green;
                            oText.color = Color.red;
                            break;
                    }
                    break;
                case RPS.PAPER:
                    switch (opponentMoves[i].selectedMove)
                    {
                        case RPS.ROCK:
                            wins++;
                            aText.color = Color.green;
                            oText.color = Color.red;
                            break;
                        case RPS.PAPER:
                            ties++;
                            aText.color = Color.yellow;
                            oText.color = Color.yellow;
                            break;
                        case RPS.SCISSOR:
                            losses++;
                            aText.color = Color.red;
                            oText.color = Color.green;
                            break;
                    }
                    break;
                case RPS.SCISSOR:
                    switch (opponentMoves[i].selectedMove)
                    {
                        case RPS.ROCK:
                            losses++;
                            aText.color = Color.red;
                            oText.color = Color.green;
                            break;
                        case RPS.PAPER:
                            wins++;
                            aText.color = Color.green;
                            oText.color = Color.red;
                            break;
                        case RPS.SCISSOR:
                            ties++;
                            aText.color = Color.yellow;
                            oText.color = Color.yellow;
                            break;
                    }
                    break;
            }
        }

        winsText.color = Color.green;
        winsText.text = $"Wins: {wins}";

        tiesText.color = Color.green;
        tiesText.text = $"Ties: {ties}";

        lossesText.color = Color.red;
        lossesText.text = $"Losses: {losses}";

    }

    private static RPSLoader GetInstance()
    {
        if (instance != null)
            return instance;

        instance = new GameObject("GameLoader Manager").AddComponent<RPSLoader>();
        return instance;
    }
}
