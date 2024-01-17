using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

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

    public static async Task<RegResult> AttemptRegister(RegisterData newUserData)
    {
        RegResult result = new RegResult();

        // TODO:
        // Check if username is vialbe
        // Check if username already in the database
        // Check if password is secure, properly

        // Check all fields being filled
        if (newUserData.username == ""
            || newUserData.password == ""
            || newUserData.passwordRepeat == ""
            || newUserData.email == "")
        {
            result.errorMsg = "Please awnser all fields";
            return result;
        }

        // Check if the password entries match
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

        // Check if the username length is ok
        if(newUserData.username.ToCharArray().Length < 4)
        {
            result.errorMsg = "Username too short";
            return result;
        }

        if (newUserData.username.ToCharArray().Length > 14)
        {
            result.errorMsg = "Username too long";
            return result;
        }

        // Check for disallowed symbols
        if (   newUserData.username.Contains('!')
            || newUserData.username.Contains(',')
            || newUserData.username.Contains(':')
            || newUserData.username.Contains('{')
            || newUserData.username.Contains('}')
            || newUserData.username.Contains('"')
            || newUserData.username.Contains('?')
            || newUserData.username.Contains('<')
            || newUserData.username.Contains('>')
            || newUserData.username.Contains('/')
            || newUserData.username.Contains('|')
            )
        {
            result.errorMsg = "Username contains disallowed symbols";
            return result;
        }

        // Check if the username is available
        string jsonBlorb = await FirebaseLoader.LoadTable("userNames");
        if (jsonBlorb != null)
        {
            char[] parms = { ',', ':', '{', '}', '"' };
            List<string> existingUserNames = jsonBlorb.Split(parms, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (existingUserNames.Contains(newUserData.username.Trim()))
            {
                result.errorMsg = "Username not available";
                return result;
            }
        }
        #region frunctional alternative
        /*var db = FirebaseDatabase.DefaultInstance;
        await db.GetReference("userNames").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                //Debug.LogError(task.Exception);
            }

            //here we get the result from our database.
            DataSnapshot snap = task.Result;
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var item in snap.Children)
            {
                result.Add(item.Key, item.Value.ToString());
                Debug.Log(item.Key + ": + " + item.Value.ToString());
            }
            //And send the JSON data to a function that can update our game.
        });*/
        #endregion

        // Attempt to create the account
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

        if(result.sucess)
           await Login.AttemptLogin(newUserData.email, newUserData.password, true);

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