using Firebase.Auth;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public struct RegResult
{
    RegResult(bool success = false, string errorMsg = "")
    {
        this.sucess = success;
        this.errorMsg = errorMsg;
    }

    public bool sucess;
    public string errorMsg;
}

public static class Register
{
    public static string[] insecurePasswords = { "123456789", "987654321", "password", "password123", "admin", "drowssap", "abcdef", "abc123", "123abc" };

    // TODO: Refactor, currently using async but returns as sucess before that resolved
    public static async Task<RegResult> AttemptRegister(RegisterData newUserData)
    {
        RegResult result = new RegResult();

        // TODO:
        // Check if username is vialbe
        // Check if username already in the database
        // Check if password is secure, properly

        if (newUserData.username == ""
            || newUserData.password == ""
            || newUserData.passwordRepeat == ""
            || newUserData.email == "")
        {
            result.errorMsg = "Please awnser all fields";
            return result;
        }

        if(newUserData.password != newUserData.passwordRepeat)
        {
            result.errorMsg = "Passwords do not match";
            return result;
        }

        if (insecurePasswords.Contains(newUserData.password))
        {
            result.errorMsg = "Insecure Password";
            return result;
        }

        if(newUserData.username.ToCharArray().Length < 4)
        {
            result.errorMsg = "Username too short";
            return result;
        }

        if (newUserData.username.Contains('!'))
        {
            result.errorMsg = "Username contains disallowed symbols";
            return result;
        }

        await FirebaseInitializer.Auth.CreateUserWithEmailAndPasswordAsync(newUserData.email, newUserData.password).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                result.errorMsg = task.Exception.ToString();
                result.sucess = false;
            }
            else
            {
                FirebaseUser newAuthUser = task.Result.User;
                result.sucess = true;

                string uniqueUsername = newUserData.username/* + "!" + Guid.NewGuid().ToString()*/;

                User newUser = new User(newUserData.email, uniqueUsername);
                string userJson = JsonUtility.ToJson(newUser);
                FirebaseSaver.SaveToDatabase("users", newAuthUser.UserId, userJson);
            }
        });
        return result;
    }
}

public struct RegisterData
{
    public RegisterData(string _username, string _pw, string _pwRepeat, string _email)
    {
        username = _username;
        password = _pw;
        passwordRepeat = _pwRepeat;
        email = _email;
    }

    public string username;
    public string password;
    public string passwordRepeat;
    public string email;
}