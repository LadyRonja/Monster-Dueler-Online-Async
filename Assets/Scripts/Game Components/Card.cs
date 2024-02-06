using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Card : MonoBehaviour
{
    public string cardName;
    public string description;
    public int power;
    public int initiative;
    public Sprite background;
    public Sprite illustration;
    public bool discardable;

    public abstract void ExecuteCard();
}
