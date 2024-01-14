using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterScreen : MonoBehaviour
{
    public static RegisterScreen Instance;

    public TMP_InputField usernameInputField;
    public TMP_InputField passwordFirstInputField;
    public TMP_InputField passwordSecondInputField;
    public TMP_InputField emailInputField;
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

    public void AttemptRegister()
    {
        RegisterData regData = new RegisterData(
                                        usernameInputField.text, 
                                        passwordFirstInputField.text, 
                                        passwordSecondInputField.text, 
                                        emailInputField.text);

        if(Register.AttemptRegister(regData, out string errorMessage))
        {
            MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.loginScreen);
            LoginScreen.Instance.userInputField.text = regData.username;
            LoginScreen.Instance.passwordInputField.text = regData.password;
        }
        else
        {
            errorText.text = errorMessage;
        }
    }
}
