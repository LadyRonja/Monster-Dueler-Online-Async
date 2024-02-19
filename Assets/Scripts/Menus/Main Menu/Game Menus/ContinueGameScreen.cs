using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContinueGameScreen : MonoBehaviour
{
    public Transform continuePanel;
    public GameObject loadGameButtonPrefab;

    private void OnEnable()
    {
        DestroyButtons();
        LoadButtonsForGameContinues();
    }


    private async void LoadButtonsForGameContinues()
    {
        Debug.Log("Loading active RPS games");

        // Should only fetch games the active user is in to start with
        List<Game> gamesWithUser = await FirebaseLoader.GetGamesWithUser(ActiveUser.CurrentActiveUser, true);

        foreach (Game g in gamesWithUser)
        {
            GameObject continueObj = Instantiate(loadGameButtonPrefab, continuePanel);
            Button continueBtn = continueObj.GetComponent<Button>();
            string otherUserName;
            if (g.playerA != ActiveUser.CurrentActiveUser.username)
                otherUserName = g.playerA;
            else
                otherUserName = g.playerB;
            continueBtn.GetComponentInChildren<TMP_Text>().text = otherUserName;
            continueBtn.onClick.AddListener(delegate () { SceneHandler.LoadRPSGame(g.gameID); });
        }
    }

    private void DestroyButtons()
    {
        // Clear old buttons
        foreach (Transform child in continuePanel)
        {
            Destroy(child.gameObject);
        }
    }
}
