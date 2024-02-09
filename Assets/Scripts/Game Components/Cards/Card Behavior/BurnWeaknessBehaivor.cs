using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnWeaknessBehaivor : CardBehaivor
{
    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;

        List<Monster> targetTeam = new();
        if (RotationHandler.Instance.activeUserMonsters.Contains(user)) targetTeam = RotationHandler.Instance.activeUserMonsters;
        else targetTeam = RotationHandler.Instance.opponentMonsters;

        int damage = 0;
        foreach (Monster m in targetTeam)
        {
            damage += m.damageCounters;
            damage += m.slowingCounters;

            m.damageCounters = 0;
            m.slowingCounters = 0;
        }

        target.TakeDamage(damage, card.myType, user, target);
    }
}
