using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirebaseLoader
{
   public static async Task<string> LoadFromDatabase(string loadFromTable, string loadFromPath)
   {
        string output = "";
        var db = FirebaseDatabase.DefaultInstance;
        await db.RootReference.Child(loadFromTable).Child(loadFromPath).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
               //Debug.LogError(task.Exception);
            }

            //here we get the result from our database.
            DataSnapshot snap = task.Result;

            //And send the JSON data to a function that can update our game.
            output = snap.GetRawJsonValue();
        });
        return output;
   }

    public static async Task<string> LoadTable(string tableName)
    {
        string output = "";

        var db = FirebaseDatabase.DefaultInstance;
        await db.GetReference(tableName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                //Debug.LogError(task.Exception);
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
