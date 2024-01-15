using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Threading.Tasks;

public class FirebaseLoader
{
   private async Task<string> LoadFromDatabase()
   {
        string output = "";
        var db = FirebaseDatabase.DefaultInstance;
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        await db.RootReference.Child("users").Child(userId).GetValueAsync().ContinueWithOnMainThread(task =>
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
}
