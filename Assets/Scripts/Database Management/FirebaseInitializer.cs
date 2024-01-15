using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Database;

public class FirebaseInitializer : MonoBehaviour
{
    private static FirebaseInitializer instance;

    private FirebaseAuth auth;
    private FirebaseDatabase db;

    public static FirebaseInitializer Instance { get => _GetInstance(); private set => instance = value; }
    public static FirebaseAuth Auth { get => _GetAuth(); private set => Instance.auth = value; }
    public static FirebaseDatabase Db { get => _GetDB(); private set => Instance.db = value; }

    void Awake()
    {
        if (instance == null || instance == this)
            instance = this;
        else
            Destroy(this);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
                Debug.LogError(task.Exception);

            Instance.auth = FirebaseAuth.DefaultInstance;
            Instance.db = FirebaseDatabase.DefaultInstance;

            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        });
    }

    private static FirebaseInitializer _GetInstance()
    {
        if(instance != null)
            return instance;

        GameObject newInstance = new GameObject("Firebase Initializer");
        return newInstance.AddComponent<FirebaseInitializer>();
    }

    private static FirebaseAuth _GetAuth()
    {
        return Instance.auth;
    }

    private static FirebaseDatabase _GetDB()
    {
        return Instance.db;
    }
}
