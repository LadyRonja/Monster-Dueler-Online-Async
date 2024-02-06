using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageBehaivor : CardBehaivor
{
    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");
        
        if (userIsTarget)
            target = user;

        target.TakeDamage(card.power, card.myType, user, target);
    }
}
