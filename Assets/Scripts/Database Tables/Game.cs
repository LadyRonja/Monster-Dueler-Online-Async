
using System;
using System.Collections.Generic;

[Serializable]
public class Game
{
    public string gameID = Guid.NewGuid().ToString();
    public int openSlots = 2;
    public bool gameIsOver = false;

    public List<User> players = new();
    public List<GameData> gameDatas= new();

    public Game(int openSlots, List<User> players, List<GameData> gameDatas) 
    { 
        gameID = Guid.NewGuid().ToString();
        this.openSlots = openSlots;
        this.players = players;
        this.gameDatas = gameDatas;
    }
}
