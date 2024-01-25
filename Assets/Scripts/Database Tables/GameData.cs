
using System.Collections.Generic;

public class GameData 
{
    public string dataID; // ID
    public string dataOwner; // composite key
    public string forGame; // composite key

    public List<Monster> monsters = new();
    public List<Card> hand = new();
    public List<Card> deck = new();
    public List<Card> discard = new();

    public List<int> diceResults = new();
    public int movesPlayed = 0;
    public Card playedCard;
    public Card discardedCard;
}
