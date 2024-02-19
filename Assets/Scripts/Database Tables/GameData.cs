
using System;
using System.Collections.Generic;

[Serializable]
public class GameData 
{
    public string dataID; // ID
    public string dataOwner; // composite key
    public string forGame; // composite key

    public List<Monster> monsters = new();
    public List<CardKey> startHand = new();
    public List<CardKey> startDeck = new();

    public List<GameMove> moves = new();
}
