using System;
using System.Collections.Generic;

[Serializable]
public class RPSGame
{
    public string gameID = Guid.NewGuid().ToString();
    public int openSlots = 2;
    public bool gameIsOver = false;
    public string winnner = "";
    public long gameDoneAt = 0;

    public string playerA;
    public string playerB;
    public List<RPSMove> playerAMoves;
    public List<RPSMove> playerBMoves;

    public RPSGame(int openSlots, List<User> players)
    {
        gameID = Guid.NewGuid().ToString();
        this.openSlots = openSlots;
        this.playerA = players[0].username;
        this.playerB = players[1].username;
    }
}
