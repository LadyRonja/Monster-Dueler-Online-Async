using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageImmunityBehaivor : CardBehaivor
{
    public int durationToAdd;

    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;

        target.immuneForTurns += durationToAdd;
    }
}
