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
        firstStart = false;
    }

    private void PerformContinuedStarts()
    {
        MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);

    }
}
