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
        await AttemptToCreateNewGame();
    }

    public async Task AttemptToCreateNewGame()
    {
        Debug.Log("starting, searching for user: " + otherUserInputField.text);

        if(otherUserInputField.text == ActiveUser.CurrentActiveUser.username)
        {
            // TODO: Tell the user they cant play vs themselves
            Debug.Log("You can not play the game vs yourself");
            return;
        }

        string userBlob = await FirebaseLoader.LoadByValue(DBPaths.USER_TABLE, "username", otherUserInputField.text);
        User otherPlayer = JsonUtility.FromJson<User>(userBlob);

        if (string.IsNullOrEmpty(userBlob) || otherPlayer == null)
        {
            // TODO: Display error to user
            Debug.Log("No user with that username was found");
            return;
        }

        List<User> players = new() {ActiveUser.CurrentActiveUser, otherPlayer};
        List<Monster> monsters = new() {
            new Monster(10, Element.Fire, Position.Front),
            new Monster(15, Element.Water, Position.BackLeft),
            new Monster(12, Element.Grass, Position.BackRight)
        };

        // TODO: Shuffle deck and deal hand
        List<Card> deck = new() {
            new Card(),
            new Card()};
        deck[0].id = 0;
        deck[1].id = 1;

        List<Card> hand = new() {
            new Card(),
            new Card()};
        deck[0].id = 2;
        deck[1].id = 3;

        GameData gameData1 = new GameData();
        gameData1.deck = deck;
        gameData1.hand = hand;
        gameData1.monsters = monsters;

        GameData gameData2 = gameData1;

        List<GameData> gameDatas = new() { gameData1, gameData2 };

        Game newGame = new Game(0, players, gameDatas);
        newGame.gameDatas[0].dataOwner = ActiveUser.CurrentActiveUser.username;
        newGame.gameDatas[0].forGame = newGame.gameID;
        newGame.gameDatas[0].dataID = newGame.gameDatas[0].dataOwner + "_" + newGame.gameDatas[0].forGame;

        newGame.gameDatas[1].dataOwner = otherPlayer.username;
        newGame.gameDatas[1].forGame = newGame.gameID;
        newGame.gameDatas[1].dataID = newGame.gameDatas[1].dataOwner + "_" + newGame.gameDatas[1].forGame;

        string jsonGameBlob = JsonUtility.ToJson(newGame);

        Debug.Log(newGame.gameDatas.Count);
        Debug.Log(jsonGameBlob);

        FirebaseSaver.SaveToDatabase(DBPaths.GAMES_TABLE, newGame.gameID, jsonGameBlob);

        Debug.Log("Game Created?");
    }
}
