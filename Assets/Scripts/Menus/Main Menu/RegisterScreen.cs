using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterScreen : MonoBehaviour
{
    public static RegisterScreen Instance;

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordFirstInputField;
    public TMP_InputField passwordSecondInputField;
    public TMP_InputField emailInputField;
    [Space]
    public Button registerButton;
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

    public async void AttemptRegister()
    {
        registerButton.interactable = false;

        RegisterData regData = new RegisterData(
                                        usernameInputField.text, 
                                        passwordFirstInputField.text, 
                                        passwordSecondInputField.text, 
                                        emailInputField.text);

        var registerResult = await Register.AttemptRegister(regData);

        if(registerResult.sucess)
        {
            MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.loginScreen);
            LoginScreen.Instance.emailInputField.text = regData.email;
            LoginScreen.Instance.passwordInputField.text = regData.password;
        }
        else
        {
            errorText.text = registerResult.errorMsg;
        }

        registerButton.interactable = true;
    }
}
