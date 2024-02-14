using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour
{
    // Singleton
    private static GameLoader instance;
    public static GameLoader Instance { get => GetInstance(); private set => instance = value; }

    public static string gameIDToLoad = "";
    // Development purpose
    string debugGame = "e8e74f62-9097-41b4-a08e-b87b67af2e68";
    bool usingDebug = true;

    // Visual Representation
    public GameDataRepresentor activePlayerRepresentor;
    public GameDataRepresentor opponentRepresentor;
    public GameObject monsterDisplayPrefab;
    public GameObject moveButtonPrefab;

    // Card key to card translation
    [SerializedDictionary("ID, Card")]
    public SerializedDictionary<int, GameObject> cardPrefabDictionary;

    // Player data
    public GameData activePlayerData;
    public GameData opponentPlayerData;
    public List<GameMove> activePlayerMoves = new();
    public List<GameMove> opponentMoves = new();


    private void Awake()
    {
        #region Singleton
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this.gameObject);
        #endregion

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
        {
            gameBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.GAMES_TABLE, debugGame);
            string activeUserBlob = await FirebaseLoader.LoadFromDatabase(DBPaths.USER_TABLE, "9GZbdeUZ5PPlkkmZAJMgJPvKoDj2");
            ActiveUser.SetActiveUser(JsonUtility.FromJson<User>(activeUserBlob));
        }
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

        GameData activePlayerData = gameToLoad.gameDatas[0];
        GameData opponentData = gameToLoad.gameDatas[1];

        // Ensure the right player data is assigned the right data
        if(activePlayerData.dataOwner != ActiveUser.CurrentActiveUser.username)
        {
            GameData swapStore = activePlayerData;
            activePlayerData = opponentData;
            opponentData = swapStore;
        }

        LoadPlayerData(activePlayerData, activePlayerRepresentor, true);
        LoadPlayerData(opponentData, opponentRepresentor, false);
    }

    public void LoadPlayerData(GameData dataToLoad, GameDataRepresentor dataRepresentor, bool loadingForActivePlayer)
    {
        // Load username
        dataRepresentor.usernameText.text = dataToLoad.dataOwner;

        // Load options in hand here
        if(loadingForActivePlayer)
        {
            // Clear old hand items
            foreach (Transform child in dataRepresentor.handContainer)
            {
                Destroy(child.gameObject);
            }

            // Add fixed rotate cards
            AddCardToHandContainer(100, dataRepresentor.handContainer);
            AddCardToHandContainer(101, dataRepresentor.handContainer);

            // Add variable cards
            for (int i = 0; i < dataToLoad.hand.Count; i++)
            {
                AddCardToHandContainer(dataToLoad.hand[i].id, dataRepresentor.handContainer);
            }
        }

        // Load Cards in discard

        // Load Monsters
        for (int i = 0; i < dataToLoad.monsters.Count; i++)
        {
            GameObject monsterDisplayObj = Instantiate(monsterDisplayPrefab, dataRepresentor.monsterContainerFront);
            MonsterDisplay monsterDisplayScr = monsterDisplayObj.GetComponent<MonsterDisplay>();

            monsterDisplayScr.healthText.text = $"HP: {dataToLoad.monsters[i].curHealth} / {dataToLoad.monsters[i].maxHealth}";
            switch (dataToLoad.monsters[i].myElement)   
            {
                case Element.Fire:
                    monsterDisplayScr.background.color = Color.red;
                    break;
                case Element.Water:
                    monsterDisplayScr.background.color = Color.blue;
                    break;
                case Element.Grass:
                    monsterDisplayScr.background.color = Color.green;
                    break;
                default:
                    Debug.LogError("dataToLoad.monsters[i].myElement reached end of switch");
                    break;
            }

            switch (dataToLoad.monsters[i].myPosition)
            {
                case Position.Front:
                    monsterDisplayObj.transform.parent = dataRepresentor.monsterContainerFront;
                    monsterDisplayObj.transform.position = dataRepresentor.monsterContainerFront.position;
                    break;
                case Position.BackRight:
                    monsterDisplayObj.transform.parent = dataRepresentor.monsterContainerBackRight;
                    monsterDisplayObj.transform.position = dataRepresentor.monsterContainerBackRight.position;
                    break;
                case Position.BackLeft:
                    monsterDisplayObj.transform.parent = dataRepresentor.monsterContainerBackLeft;
                    monsterDisplayObj.transform.position = dataRepresentor.monsterContainerBackLeft.position;
                    break;
                default:
                    Debug.LogError("dataToLoad.monsters[i].myPosition reached end of switch");
                    break;
            }

            if (loadingForActivePlayer)
            {
                RotationHandler.Instance.activeUserMonsters = dataToLoad.monsters;
                activePlayerMoves = dataToLoad.moves;
                activePlayerData = dataToLoad;

            }
            else
            {
                RotationHandler.Instance.opponentMonsters = dataToLoad.monsters;
                opponentMoves = dataToLoad.moves;
                opponentPlayerData = dataToLoad;
            }
        }
    }

    private void AddCardToHandContainer(int dictionaryIndex, Transform handContainer)
    {
        GameObject cardObj = Instantiate(moveButtonPrefab, handContainer);
        Card cardToLoad = cardPrefabDictionary[dictionaryIndex].GetComponent<Card>();
        cardToLoad.assignedID = dictionaryIndex;

        Color colorToSet = Color.grey;
        if (cardToLoad.myType == Element.Fire) colorToSet = Color.red;
        else if(cardToLoad.myType == Element.Grass) colorToSet = Color.green;
        else if (cardToLoad.myType == Element.Water) colorToSet = Color.blue;

        cardObj.GetComponent<Image>().color = colorToSet;
        cardObj.GetComponentInChildren<TMP_Text>().text = cardToLoad.cardName;
        cardObj.GetComponent<Button>().onClick.AddListener(delegate () { cardToLoad.SetUpdisplay(MoveManager.Instance.cardDisplayer); }) ;
    }

    private static GameLoader GetInstance()
    {
        if(instance != null)
            return instance;

        instance = new GameObject("GameLoader Manager").AddComponent<GameLoader>();
        return instance;
    }
}
