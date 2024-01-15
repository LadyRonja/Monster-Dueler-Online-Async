using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginScreen : MonoBehaviour
{
    public static LoginScreen Instance;

    public TMP_InputField emailInputField;
    public TMP_InputField passwordInputField;
    [Space]
    public Button loginButton;
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

    public async void AttemptLogin()
    {
        loginButton.interactable = false;
        var loginResult = await Login.AttemptLogin(emailInputField.text, passwordInputField.text);
        if (loginResult.success)
        {
            errorText.text = "";
            MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);
        }
        else
        {
            errorText.text = loginResult.errorMsg;
        }
        loginButton.interactable = true;
    }
}
