using System;
using System.Collections.Generic;

[Serializable]
public class GameMove
{
    public CardKey playedCard = null;
    public CardKey discardedCard = null;
    public List<CardKey> discard = new();
    public List<CardKey> hand = new();
    public List<CardKey> deck = new();
    public List<Monster> monstersState = new();
    public float initiativeTie = 0;
}
