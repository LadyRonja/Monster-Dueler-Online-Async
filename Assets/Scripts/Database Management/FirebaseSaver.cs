
using Firebase.Auth;
using Firebase.Database;

public class FirebaseSaver
{
    public static async void SaveToDatabase(string saveToTable, string saveToPath, string jsonString)
    {
        var db = FirebaseInitializer.db;
        if (FirebaseInitializer.auth.CurrentUser == null)
            return;

        //puts the JSON data in the "saveToTable/saveToPath" part of the database.
        await db.RootReference.Child(saveToTable).Child(saveToPath).SetRawJsonValueAsync(jsonString);
    }

    public static async void SaveValueToDatabase(string saveToTable, string saveToPath, string value)
    {
        var db = FirebaseInitializer.db;
        if (FirebaseInitializer.auth.CurrentUser == null)
            return;

        //puts the JSON data in the "saveToTable/saveToPath" part of the database.
        await db.RootReference.Child(saveToTable).Child(saveToPath).SetValueAsync(value);
    }
}
