
using Firebase.Auth;
using Firebase.Database;

public class FirebaseSaver
{
    public static async void SaveToDatabase(string saveToTable, string saveToPath, string jsonString)
    {
        var db = FirebaseInitializer.Db;
        if (FirebaseInitializer.Auth.CurrentUser == null)
            return;

        //puts the JSON data in the "saveToTable/saveToPath" part of the database.
        await db.RootReference.Child(saveToTable).Child(saveToPath).SetRawJsonValueAsync(jsonString);
    }
}
