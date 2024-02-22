using Firebase.Database;
using System;
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
    public const int MAX_MOVES = 10;
    // Development purpose
    public string debugGame = "339fd4bf-c9be-4468-ab2c-304f41516327";
    public bool usingDebug = true;
    [Space(20)]
    [SerializeField] GameObject moveEntryPrefab;
    [SerializeField] Transform activePlayerEntryParent;
    [SerializeField] Transform opponentPlayerEntryParent;
    [Space]
    [SerializeField] TMP_Text activePlayerNameText;
    [SerializeField] TMP_Text opponentNameText;
    [Space]
    [SerializeField] TMP_Text winsText;
    [SerializeField] TMP_Text tiesText;
    [SerializeField] TMP_Text lossesText;
    [Space]
    [SerializeField] TMP_Text victoryText;

    [HideInInspector]
    public RPSGame loadedGame;
    public List<RPSMove> activePlayerMoves;
    public List<RPSMove> opponentMoves;

    public delegate void LoadedGame();
    public LoadedGame OnStateUpdated;
    public LoadedGame OnGameLoaded;

    enum MoveResult {UNDETERMINED, WON, TIED, LOST }
    List<(RPS myMove, List<RPS> opponentsMove, MoveResult result)> rules = new();

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

        CreateRules();

        Invoke(nameof(FetchGame), 1);
        Invoke(nameof(Subscribe), 1);
    }

    private void CreateRules()
    {
        rules = new();

        // Soldier
        rules.Add((RPS.SOLDIER,
                    new() {
                        RPS.POLITICIAN, 
                        RPS.FIREBALL},
                            MoveResult.LOST));

        rules.Add((RPS.SOLDIER,
            new() {
                        RPS.ASSASSIN,
                        RPS.COUNTER_SPELL},
                    MoveResult.WON));

        // Assassin
        rules.Add((RPS.ASSASSIN,
            new() {
                        RPS.SOLDIER,
                        RPS.FIREBALL},
                    MoveResult.LOST));

        rules.Add((RPS.ASSASSIN,
            new() {
                        RPS.POLITICIAN,
                        RPS.COUNTER_SPELL},
                    MoveResult.WON));

        // Politician
        rules.Add((RPS.POLITICIAN,
            new() {
                        RPS.ASSASSIN,
                        RPS.FIREBALL},
                    MoveResult.LOST));

        rules.Add((RPS.POLITICIAN,
            new() {
                        RPS.SOLDIER,
                        RPS.COUNTER_SPELL},
                    MoveResult.WON));

        // Fireball
        rules.Add((RPS.FIREBALL,
            new() {
                        RPS.ASSASSIN,
                        RPS.POLITICIAN,
                        RPS.SOLDIER},
                    MoveResult.WON));

        rules.Add((RPS.FIREBALL,
            new() {
                        RPS.COUNTER_SPELL},
                    MoveResult.LOST));

        // Counter Spell
        rules.Add((RPS.COUNTER_SPELL,
            new() {
                        RPS.FIREBALL},
                    MoveResult.WON));

        rules.Add((RPS.COUNTER_SPELL,
            new() {
                        RPS.ASSASSIN,
                        RPS.POLITICIAN,
                        RPS.SOLDIER},
                    MoveResult.LOST));

    }
    private void Subscribe()
    {
        string idToTack = rpsGameToLoad;
        if (usingDebug) idToTack = debugGame;

        FirebaseInitializer.db.GetReference($"{DBPaths.RPS_TABLE}/{idToTack}").ValueChanged += HandleValueChange;
    }

    private void HandleValueChange(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            //Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Do something with the data in args.Snapshot
        Debug.Log("Value has changed: " + args.Snapshot.GetRawJsonValue());

        //run the game with the new information
        FetchGame();
    }


    public async void FetchGame()
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

        foreach (Transform child in activePlayerEntryParent)
            Destroy(child.gameObject);
        foreach (Transform child in opponentPlayerEntryParent)
            Destroy(child.gameObject);


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
            activePlayerNameText.text = gameToLoad.playerA;
            opponentNameText.text = gameToLoad.playerB;
        }
        else
        {
            activePlayerMoves = gameToLoad.playerBMoves;
            opponentMoves = gameToLoad.playerAMoves;
            activePlayerNameText.text = gameToLoad.playerB;
            opponentNameText.text = gameToLoad.playerA;
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

            MoveResult result = MoveResult.UNDETERMINED;

            foreach (var rule in rules)
            {
                if (activePlayerMoves[i].selectedMove != rule.myMove)
                    continue;

                if (rule.opponentsMove.Contains(opponentMoves[i].selectedMove))
                {
                    result = rule.result; 
                    break;
                }
            }

            switch (result)
            {
                case MoveResult.WON:
                    wins++;
                    aText.color = Color.green;
                    oText.color = Color.red;
                    break;
                case MoveResult.LOST:
                    losses++;
                    aText.color = Color.red;
                    oText.color = Color.green;
                    break;
                case MoveResult.UNDETERMINED:
                case MoveResult.TIED:
                    ties++;
                    aText.color = Color.yellow;
                    oText.color = Color.yellow;
                    break;
            }
        }

        if (activePlayerMoves.Count == opponentMoves.Count+1) 
        {
            GameObject aMove = Instantiate(moveEntryPrefab, activePlayerEntryParent);
            TMP_Text aText = aMove.GetComponentInChildren<TMP_Text>();
            aText.text = ((RPS)activePlayerMoves[^1].selectedMove).ToString();
        }

        winsText.color = Color.green;
        winsText.text = $"Wins: {wins}";

        tiesText.color = Color.yellow;
        tiesText.text = $"Ties: {ties}";

        lossesText.color = Color.red;
        lossesText.text = $"Lost: {losses}";

        loadedGame = gameToLoad;

        if (IsGameOver(movesToLoad))
        {
            MoveResult winState = MoveResult.TIED;
            if (wins > losses) winState = MoveResult.WON;
            else if (wins < losses) winState = MoveResult.LOST;
            CompleteGame(gameToLoad, winState);
        }

        OnGameLoaded?.Invoke();

    }

    private bool IsGameOver(int movesToLoad)
    {
        if (movesToLoad >= MAX_MOVES)
            return true;
        else 
            return false;
    }

    private void CompleteGame(RPSGame gameToComplete, MoveResult activePlayerWon)
    {
        gameToComplete.gameIsOver = true;
        if(gameToComplete.gameDoneAt == 0) gameToComplete.gameDoneAt = DateTime.Now.Ticks;
        string gameBlob = JsonUtility.ToJson(gameToComplete);
        FirebaseSaver.SaveToDatabase(DBPaths.RPS_TABLE, gameToComplete.gameID, gameBlob);
        loadedGame = gameToComplete;

        switch (activePlayerWon)
        {
            case MoveResult.WON:
                victoryText.text = "YOU WON!";
                gameToComplete.winnner = ActiveUser.CurrentActiveUser.username;
                victoryText.color = Color.green;
                break;
            case MoveResult.TIED:
                victoryText.text = "TIE";
                gameToComplete.winnner = "";
                victoryText.color = Color.yellow;
                break;
            case MoveResult.LOST:
                victoryText.text = "YOU LOST!";
                if (ActiveUser.CurrentActiveUser.username.Equals(gameToComplete.playerA)) gameToComplete.winnner = gameToComplete.playerB;
                else gameToComplete.winnner = gameToComplete.playerA;
                victoryText.color = Color.red;
                break;
        }

        victoryText.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        OnDestroy();
    }

    private void OnDestroy()
    {
        string idToTack = rpsGameToLoad;
        if (usingDebug) idToTack = debugGame;

        FirebaseInitializer.db.GetReference($"{DBPaths.RPS_TABLE}/{idToTack}").ValueChanged -= HandleValueChange;
    }

    private static RPSLoader GetInstance()
    {
        if (instance != null)
            return instance;

        instance = new GameObject("GameLoader Manager").AddComponent<RPSLoader>();
        return instance;
    }
}
