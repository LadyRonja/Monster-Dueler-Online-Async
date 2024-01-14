using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuScreen : MonoBehaviour
{
    public static MainMenuScreen Instance;

    public GameObject mainMenuScreen;
    public GameObject playScreen;
    public GameObject newGameScreen;
    public GameObject continueGameScreen;

    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void OnEnable()
    {
        SetScreenActive(mainMenuScreen);
    }

    public void LogOut()
    {
        Login.Logout();
        MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.loginScreen);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void SetScreenActive(GameObject screenToActivate)
    {
        mainMenuScreen.SetActive(false);
        playScreen.SetActive(false);
        newGameScreen.SetActive(false);
        continueGameScreen.SetActive(false);

        screenToActivate.SetActive(true);
    }
}
