using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
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
        string userBlob = await FirebaseLoader.LoadByValue(DBPaths.USER_TABLE, "username", otherUserInputField.text);
        User otherPlayer = JsonUtility.FromJson<User>(userBlob);

        if (string.IsNullOrEmpty(userBlob) || otherPlayer == null)
        {
            // TODO: Display error to user
            Debug.Log("No user with that username was found");
            return;
        }

        List<User> players = new() {ActiveUser.CurrentActiveUser, otherPlayer};

    }
}
