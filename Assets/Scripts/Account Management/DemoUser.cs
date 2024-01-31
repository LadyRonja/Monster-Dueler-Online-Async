using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class creates test users for Firebase Authentication
/// Just add this script to a button and make as many copies
/// of the button as you want users.
/// </summary>

[RequireComponent(typeof(Button))]
public class DemoUser : MonoBehaviour
{
    int currentUser = 0;

    FirebaseAuth auth;

    void Start()
    {
        //Set the current user
        var demoUserButtons = GameObject.FindObjectsOfType<DemoUser>();

        for (int i = 0; i < demoUserButtons.Length; i++)
        {
            if (demoUserButtons[i] == this)
                currentUser = i;
        }

        //Update the text on the button to show which user this is
        GetComponentInChildren<TextMeshProUGUI>().text = "Test User " + currentUser;

        //Add a listener to the button to call the SignInTestUser function when clicked
        GetComponent<Button>().onClick.AddListener(delegate { SignInTestUser(); });

        //Make sure Firebase is initialized
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            auth = FirebaseAuth.DefaultInstance;
        });
    }

    void SignInTestUser()
    {
        //Create a new user with a unique email address
        string username = "test" + currentUser + "@test.test";
        string password = "pass123";

        SignIn(username, password);
    }

    private async Task RegisterNewUser(string email, string password)
    {
        string username = email.Substring(0, email.Length - 10);
        RegisterData newRegData = new RegisterData(username, password, password, email);
        await Register.AttemptRegister(newRegData);
    }

    private async void SignIn(string email, string password)
    {
        //We need to sign out to change users
        if (auth.CurrentUser != null && auth.CurrentUser.Email != email)
            auth.SignOut();
        else if (auth.CurrentUser != null)
            LoadNextScene(email); //Already signed in, we can load next scene

        LoginResult logRes = await Login.AttemptLogin(email, password);
        if (logRes.success)
            LoadNextScene(email);
        else
        {
            LoginScreen.Instance.errorText.text = logRes.errorMsg;
            await RegisterNewUser(email, password);
            SignInTestUser();
        }
    }

    private void LoadNextScene(string devMail)
    {
        MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);
        MainMenuScreen.Instance.greetingText.text = $"Logged in as dev {devMail}";

        Invoke("UpdateAcitveUser", 1);
    }

    private async void UpdateAcitveUser()
    {
        string userblob = await FirebaseLoader.LoadFromDatabase(DBPaths.USER_TABLE, FirebaseInitializer.auth.CurrentUser.UserId);
        ActiveUser.SetActiveUser(JsonUtility.FromJson<User>(userblob));
        MainMenuScreen.Instance.greetingText.text = $"Logged in as dev {ActiveUser.CurrentActiveUser.username}";
    }
}
