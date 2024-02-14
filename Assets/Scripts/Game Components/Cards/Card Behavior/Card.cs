using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    public string cardName;
    [TextArea(3, 5)]
    public string description;
    public int power;
    public int initiative;
    public Sprite background;
    public Sprite illustration;
    public Element myType;
    public bool discardable;
    public List<CardBehaivor> behaivors;

    public int assignedID = 0;

    public void SetUpdisplay(DisplayCard display)
    {
        display.gameObject.SetActive(true);
        display.nameText.text = cardName;
        display.descriptionText.text = description;
        display.powerText.text = power.ToString();
        display.initiativeText.text = initiative.ToString();
        display.background.sprite = background;
        display.illustration.sprite = illustration;

        MoveManager.Instance.SetUpConfirmButton(this);
    }

    public virtual void ExecuteCard(Monster user, Monster target)
    {
        Debug.Log($"Casting card {cardName}");

        for (int i = 0; i < behaivors.Count; i++)
        {
            behaivors[i].ExecuteBehaivor(user, target, this);
        }
    }
}
