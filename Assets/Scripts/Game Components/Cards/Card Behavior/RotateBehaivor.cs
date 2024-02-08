using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBehaivor : CardBehaivor
{
    public bool rotateCounterClockwise;

    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;

        RotationHandler.Instance.RotateTeam(target, rotateCounterClockwise);
    }
}
