using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NewGameScreen : MonoBehaviour
{
    public TMP_InputField otherUserInputField;

    public async void TEMP()
    {
        //await AttemptToCreateNewGame();
        AtemptCreateRPSGame();
    }

    public async Task AtemptCreateRPSGame()
    {
        Debug.Log("starting, searching for user: " + otherUserInputField.text);

        if (otherUserInputField.text == ActiveUser.CurrentActiveUser.username || otherUserInputField.text == string.Empty)
        {
            // TODO: Tell the user they cant play vs themselves
            Debug.Log("You can not play the game vs yourself");
            return;
        }

        User otherPlayer = await FirebaseLoader.GetUserFromUserNamer(otherUserInputField.text);

        if (otherPlayer == null)
        {
            // TODO: Display error to user
            Debug.Log("No user with that username was found");
            return;
        }

        List<User> players = new() {
            ActiveUser.CurrentActiveUser,
            otherPlayer};

        RPSGame newGame = new RPSGame(0, players);

        string jsonGameBlob = JsonUtility.ToJson(newGame);

        FirebaseSaver.SaveToDatabase(DBPaths.RPS_TABLE, newGame.gameID, jsonGameBlob);

        SceneHandler.LoadRPSGame(newGame.gameID);
    }

    public async Task AttemptToCreateNewGame()
    {
        Debug.Log("starting, searching for user: " + otherUserInputField.text);

        if(otherUserInputField.text == ActiveUser.CurrentActiveUser.username || otherUserInputField.text == string.Empty)
        {
            // TODO: Tell the user they cant play vs themselves
            Debug.Log("You can not play the game vs yourself");
            return;
        }

        User otherPlayer = await FirebaseLoader.GetUserFromUserNamer(otherUserInputField.text);

        if (otherPlayer == null)
        {
            // TODO: Display error to user
            Debug.Log("No user with that username was found");
            return;
        }

        List<User> players = new() {
            ActiveUser.CurrentActiveUser,
            otherPlayer};

        List<Monster> monsters = new() {
            new Monster(10, Element.Fire, Position.Front),
            new Monster(15, Element.Water, Position.BackLeft),
            new Monster(12, Element.Grass, Position.BackRight)
        };

        // TODO: Shuffle deck and deal hand
        List<CardKey> activeUserDeck = SetUpDeck();
        List<CardKey> activeUserHand = new();
        activeUserHand.Add(activeUserDeck[0]);
        activeUserHand.Add(activeUserDeck[1]);
        activeUserHand.Add(activeUserDeck[2]);
        activeUserHand.Add(activeUserDeck[3]);
        activeUserDeck.RemoveRange(0, 3);

        List<CardKey> opponentDeck = SetUpDeck();
        List<CardKey> opponentHand = new();
        opponentHand.Add(opponentDeck[0]);
        opponentHand.Add(opponentDeck[1]);
        opponentHand.Add(opponentDeck[2]);
        opponentHand.Add(opponentDeck[3]);
        opponentDeck.RemoveRange(0, 3);


        GameData activeUserGameData = new GameData();
        activeUserGameData.startDeck = activeUserDeck;
        activeUserGameData.startHand = activeUserHand;
        activeUserGameData.monsters = monsters;

        GameData opponentGameData = new GameData();
        opponentGameData.startDeck = opponentDeck;
        opponentGameData.startHand = opponentHand;
        opponentGameData.monsters = monsters;


        activeUserGameData.dataOwner = ActiveUser.CurrentActiveUser.username;
        opponentGameData.dataOwner = otherPlayer.username;

        List<GameData> gameDatas = new() { activeUserGameData, opponentGameData };

        Game newGame = new Game(0, players, gameDatas);
        newGame.gameDatas[0].forGame = newGame.gameID;
        newGame.gameDatas[0].dataID = newGame.gameDatas[0].dataOwner + "_" + newGame.gameDatas[0].forGame;

        newGame.gameDatas[1].forGame = newGame.gameID;
        newGame.gameDatas[1].dataID = newGame.gameDatas[1].dataOwner + "_" + newGame.gameDatas[1].forGame;

        string jsonGameBlob = JsonUtility.ToJson(newGame);

        FirebaseSaver.SaveToDatabase(DBPaths.GAMES_TABLE, newGame.gameID, jsonGameBlob);

        SceneHandler.LoadGame(newGame.gameID);
    }

    private List<CardKey> SetUpDeck()
    {
        List<CardKey> output = new();

        for (short i = 2; i < 16; i++)
        {
            CardKey newCard = new CardKey();
            newCard.id = i;
            output.Add(newCard);
        }

        for (int i = 0; i < output.Count; i++)
        {
            int rand = Random.Range(0, output.Count);
            CardKey temp = output[i];
            output[i] = output[rand];
            output[rand] = temp;
        }

        return output;
    }
}
