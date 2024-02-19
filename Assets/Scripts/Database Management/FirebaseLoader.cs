using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Google.MiniJSON;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLoader
{
   public static async Task<string> LoadFromDatabase(string loadFromTable, string loadFromPath)
   {
        string output = "";
        var db = FirebaseInitializer.db;
        await db.RootReference.Child(loadFromTable).Child(loadFromPath).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
               Debug.LogError(task.Exception);
            }

            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            //And send the JSON data to a function that can update our game.
            output = snap.GetRawJsonValue();
        });
        return output;
   }

    public static async Task<string> LoadByValue(string loadFromTable, string basedOnValue, string value)
    {
        string output = "";
        var db = FirebaseInitializer.db;
        await db.RootReference.Child(loadFromTable).OrderByChild(basedOnValue).EqualTo(value).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }

            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            //And send the JSON data to a function that can update our game.
            output = snap.GetRawJsonValue();
        });
        return output;
    }

    public static async Task<User> GetUserFromUserNamer(string userName)
    {
        User output = null;
        var db = FirebaseInitializer.db;
        await db.RootReference.Child(DBPaths.USER_TABLE).OrderByChild("username").EqualTo(userName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            if (snap.ChildrenCount > 0)
            {
                foreach (var item in snap.Children)
                {
                    string json = item.GetRawJsonValue();
                    output = JsonUtility.FromJson<User>(json);
                }
            }
        });
        return output;
    }

    public static async Task<List<Game>> GetGamesWithUser(User withUser, bool rpsGame = false)
    {
        List<Game> output = new();
        var db = FirebaseInitializer.db;

        string tableToLoad = DBPaths.GAMES_TABLE;
        if(rpsGame) tableToLoad = DBPaths.RPS_TABLE;
        await db.RootReference.Child(tableToLoad).OrderByChild("playerA").EqualTo(withUser.username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            if (snap.ChildrenCount > 0)
            {
                foreach (var item in snap.Children)
                {
                    string json = item.GetRawJsonValue();
                    output.Add(JsonUtility.FromJson<Game>(json));
                }
            }
        });

        await db.RootReference.Child(tableToLoad).OrderByChild("playerB").EqualTo(withUser.username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            if (snap.ChildrenCount > 0)
            {
                foreach (var item in snap.Children)
                {
                    string json = item.GetRawJsonValue();
                    output.Add(JsonUtility.FromJson<Game>(json));
                }
            }
        });
        return output;
    }

    public static async Task<string> LoadTable(string tableName)
    {
        string output = "";

        var db = FirebaseInitializer.db;
        await db.GetReference(tableName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError(task.Exception);
            }
            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            //And send the JSON data to a function that can update our game.
            output = snap.GetRawJsonValue();
            return output;
        });
        return output;
    }
}
