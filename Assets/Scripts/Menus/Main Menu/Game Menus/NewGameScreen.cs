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
        Debug.Log("starting");
        //string userJsonList = await FirebaseLoader.LoadTable("users");
        //Debug.Log(userJsonList);
    }
}
