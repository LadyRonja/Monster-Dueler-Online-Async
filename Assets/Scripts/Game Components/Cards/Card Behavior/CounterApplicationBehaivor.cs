using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CounterType { Damaging, Slowing}

public class CounterApplicationBehaivor : CardBehaivor
{
    public CounterType typeToApply;
    public int amountToApply;

    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;


        switch (typeToApply)
        {
            case CounterType.Damaging:
                target.damageCounters += amountToApply;
                break;
            case CounterType.Slowing:
                target.slowingCounters += amountToApply;
                break;
            default:
                Debug.LogError("Reached end of switch statement, new type declared but not defined?");
                break;
        }
    }
}
