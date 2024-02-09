using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicTacticsBehaivor : CardBehaivor
{
    public override void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log($"{user.myElement.ToString()}-mon using card: {card.cardName}, on opponent({!userIsTarget}) {target.myElement.ToString()}-mon");

        if (userIsTarget)
            target = user;

        List<Monster> potentialTargets = new();
        if (RotationHandler.Instance.opponentMonsters.Contains(target)) potentialTargets = RotationHandler.Instance.opponentMonsters;
        else potentialTargets = RotationHandler.Instance.activeUserMonsters;

        // Check if opponents last play was a basic rotate card
        bool targetFrontline = true;
        if (GameLoader.Instance.opponentMoves[^1].playedCard.id == 0 || GameLoader.Instance.opponentMoves[^1].playedCard.id == 1)
            targetFrontline = false;

        foreach (Monster m in potentialTargets)
        {
            if (targetFrontline)
            {
                if (m.myPosition == Position.Front)
                {
                    m.damageCounters += 2;
                    break;
                }
            }
            else
            {
                if (m.myPosition != Position.Front)
                {
                    m.damageCounters += 2;
                }
            }
        }
    }
}
