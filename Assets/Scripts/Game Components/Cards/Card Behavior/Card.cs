using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardName;
    public string description;
    public int power;
    public int initiative;
    public Sprite background;
    public Sprite illustration;
    public Element myType;
    public bool discardable;
    public List<CardBehaivor> behaivors;

    public virtual void ExecuteCard(Monster user, Monster target)
    {
        Debug.Log($"Casting card {cardName}");

        for (int i = 0; i < behaivors.Count; i++)
        {
            behaivors[i].ExecuteBehaivor(user, target, this);
        }
    }
}
