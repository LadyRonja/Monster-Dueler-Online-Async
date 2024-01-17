
using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;

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
            }
        });

        if (result.success)
            await LogIn();

        return result;
    }

    private static async Task LogIn()
    {
        string loadedUserJson = await FirebaseLoader.LoadFromDatabase("users", FirebaseInitializer.Auth.CurrentUser.UserId);
        User loadedUser = JsonUtility.FromJson<User>(loadedUserJson);
        FirebaseSaver.SaveValueToDatabase("userNames", Guid.NewGuid().ToString(), loadedUser.username);

        ActiveUser.SetActiveUser(loadedUser);
    }

    public static void Logout()
    {
        FirebaseInitializer.Auth.SignOut();
        ActiveUser.SetActiveUser(null);
    }
}
