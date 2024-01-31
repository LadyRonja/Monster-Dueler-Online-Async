using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string email;
    public string username;
    public List<Game> activeGames = new();
    public List<GameData> myGameData = new();

    public User(string email, string username)
    {
        this.email = email;
        this.username = username;
        this.activeGames = new();
        this.myGameData = new();
    }
}
