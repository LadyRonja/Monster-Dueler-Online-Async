
using System;
using System.Collections.Generic;

[Serializable]
public class Game
{
    public string gameID = Guid.NewGuid().ToString();
    public int openSlots = 2;
    public int turnsPlayed = 0;
    public bool gameIsOver = false;

    public string playerA;
    public string playerB;  
    public List<GameData> gameDatas;

    public Game(int openSlots, List<User> players, List<GameData> gameDatas) 
    { 
        gameID = Guid.NewGuid().ToString();
        this.openSlots = openSlots;
        this.playerA = players[0].username;
        this.playerB = players[1].username;
        this.gameDatas = gameDatas;
        this.turnsPlayed = 0;
    }
}
