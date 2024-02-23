using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupManager : MonoBehaviour
{
    public static bool firstStart = true;

    void Start()
    {
        if (firstStart)
            PerformFirstStart();
        else
            PerformContinuedStarts();
    }

    private void PerformFirstStart()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        firstStart = false;
        string lastSignIn = PlayerPrefs.GetString("lastMail");
        if(lastSignIn != null && lastSignIn != "")
        {
            LoginScreen.Instance.emailInputField.text = lastSignIn;
        }
    }

    private void PerformContinuedStarts()
    {
        MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);

    }
}
