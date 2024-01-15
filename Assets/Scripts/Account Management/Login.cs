
using Firebase.Auth;
using Firebase.Extensions;
using System.Data.Common;
using System.Threading.Tasks;

public struct LoginResult
{
    public LoginResult(bool success = false, string errorMsg = "")
    {
        this.success = success;
        this.errorMsg = errorMsg;
    }

    public bool success;
    public string errorMsg;
}

public static class Login
{
    // TODO: Refactor, currently returning before async call returns
    public static async Task<LoginResult> AttemptLogin(string email, string password)
    {
        LoginResult result = new LoginResult();
        await FirebaseInitializer.Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                result.errorMsg = task.Exception.Message;
            }
            else
            {
                FirebaseUser newUser = task.Result.User;
                result.success = true;
                LogIn();
            }
        });

        return result;
    }

    private static void LogIn()
    {
        // TODO:
        // Get user data from database

        //temp
        User signInAs = new User(FirebaseInitializer.Auth.CurrentUser.UserId, "", ""); // TODO: This is fake

        ActiveUser.SetActiveUser(signInAs);
        //MainMenuData.Instance.SetScreenActive(MainMenuData.Instance.mainMenuScreen);
    }

    public static void Logout()
    {
        ActiveUser.SetActiveUser(null);
    }
}
