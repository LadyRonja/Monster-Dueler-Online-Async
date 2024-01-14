using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginScreen : MonoBehaviour
{
    public static LoginScreen Instance;

    public TMP_InputField userInputField;
    public TMP_InputField passwordInputField;
    [Space]
    public TMP_Text errorText;

    private void Awake()
    {
        if (Instance == null || Instance == this)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
    private void OnEnable()
    {
        errorText.text = "";
    }

    public void AttemptLogin()
    {
        if(Login.AttemptLogin(userInputField.text, passwordInputField.text, out string errorMsg))
        {
            errorText.text = "";
            MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);
        }
        else
        {
            errorText.text = errorMsg;
        }
    }
}
