using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamnationOfPassivityBehaivor : CardBehaivor
{
    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;

        List<Monster> enemyMonsters = RotationHandler.Instance.opponentMonsters;
        List<Monster> allyMonsters = RotationHandler.Instance.activeUserMonsters;

        foreach (Monster m in enemyMonsters)
        {
            if (m.myPosition != Position.Front)
            {
                m.slowingCounters += 1;
                m.damageCounters += 1;
            }
        }

        foreach (Monster m in allyMonsters)
        {
            if (m.myPosition != Position.Front)
                m.damageCounters += 1;
        }

    }
}
