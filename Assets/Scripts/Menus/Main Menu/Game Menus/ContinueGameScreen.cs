using System;
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
        List<RPSGame> gamesWithUser = await FirebaseLoader.GetGamesWithUser(ActiveUser.CurrentActiveUser, true);

        foreach (RPSGame g in gamesWithUser)
        {
            GameObject continueObj = Instantiate(loadGameButtonPrefab, continuePanel);
            Button continueBtn = continueObj.GetComponent<Button>();
            string otherUserName;
            if (g.playerA != ActiveUser.CurrentActiveUser.username)
                otherUserName = g.playerA;
            else
                otherUserName = g.playerB;

            if (g.gameIsOver)
            {
                DateTime now = DateTime.Now;
                if(new DateTime(g.gameDoneAt) < now.AddDays(-7))
                {
                    Debug.Log("It should be removed");
                    Destroy(continueObj);
                    FirebaseInitializer.db.GetReference($"{DBPaths.RPS_TABLE}/{g.gameID}").RemoveValueAsync();
                    continue;
                }

                continueBtn.interactable = false;
                if(g.winnner.Equals(ActiveUser.CurrentActiveUser.username))
                {
                    continueObj.GetComponent<Image>().color = Color.green;
                }
                else if (g.winnner.Equals(""))
                {
                    continueObj.GetComponent<Image>().color = Color.yellow;
                }
                else
                {
                    continueObj.GetComponent<Image>().color = Color.red;
                }
            }
            else
            {
                continueBtn.onClick.AddListener(delegate () { SceneHandler.LoadRPSGame(g.gameID); });
            }

            continueBtn.GetComponentInChildren<TMP_Text>().text = otherUserName;
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
