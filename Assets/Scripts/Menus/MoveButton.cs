using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveButton : MonoBehaviour
{
    public Button myButton;
    public Image background;
    public Image frame;
    public Image icon;
    public bool blackIcon = true;

    public void DisableButton()
    {
        myButton.interactable = false;
        float cValue = 1;
        if(blackIcon)
            cValue= 0;

        icon.color = new Color(cValue, cValue, cValue, 0.3f);
    }

    public void EnableButton()
    {
        myButton.interactable = true;
        float cValue = 1;
        if (blackIcon)
            cValue = 0;
        icon.color = new Color(cValue, cValue, cValue, 1);
    }
}
