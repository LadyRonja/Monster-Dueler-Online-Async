
using Firebase.Auth;
using Firebase.Database;

public class FirebaseSaver
{
    public static async void SaveToDatabase(string saveToPath, string jsonString)
    {
        var db = FirebaseInitializer.Db;
        var userId = FirebaseInitializer.Auth.CurrentUser.UserId;
        //puts the JSON data in the "users/userId" part of the database.
        await db.RootReference.Child(saveToPath).Child(userId).SetRawJsonValueAsync(jsonString);
    }
}
