
using System;
using System.Collections.Generic;

[Serializable]
public class GameData 
{
    public string dataID; // ID
    public string dataOwner; // composite key
    public string forGame; // composite key

    public List<Monster> monsters = new();
    public List<CardKey> hand = new();
    public List<CardKey> deck = new();
    public List<CardKey> discard = new();

    public List<GameMove> moves = new();
}
