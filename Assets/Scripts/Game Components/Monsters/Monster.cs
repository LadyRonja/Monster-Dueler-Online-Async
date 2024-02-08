
using System;
using System.Diagnostics;
using UnityEditor.Experimental.GraphView;

public enum Element { Fire, Water, Grass, Special}
public enum Position { Front, BackRight, BackLeft}
[Serializable]
public class Monster {

    // Stats
    public int curHealth;
    public int maxHealth;
    public Element myElement;
    public Position myPosition;
    public bool alive;

    // State
    public bool justSwitchedIn;
    public int immuneForTurns = 0;

    // Counters
    public int damageCounters;
    public int slowingCounters;


    public Monster(int maxHealth, Element element, Position position)
    {
        this.maxHealth = maxHealth;
        this.curHealth = maxHealth;
        this.myElement = element;
        this.myPosition = position;

        justSwitchedIn = false;
        alive = true;
        damageCounters = 0;
        slowingCounters = 0;
    }

    public static int CalculateDamage(int damageAmount, Element damageType, Monster damageDealer, Monster damageReciever)
    {
        //Damage calculation: Card damage * STAB * type multiplier
        // If you switch into an ineffective move you heal instead of lose health
        //STAB = 2, if the attacker is the same type as the attack, otherwise
        //STAB = 1

        float output = 0;

        int sameTypeAttackBonus = (damageType == damageDealer.myElement) ? 2 : 1;
        float typeMultiplier = CalculateTypeMultiplier(damageType, damageReciever.myElement, damageReciever.justSwitchedIn);

        output = (int)MathF.Ceiling(damageAmount * sameTypeAttackBonus * typeMultiplier);


        return (int)output;
    }

    // This is a bad way of doing it,
    // A better way would be to have each type as a class
    // with a list of what types they are strong/weak against
    // or an overarching matrix informed via a visual input
    //...
    // and switching in to a super ressitent move should be calculated elsewhere
    private static float CalculateTypeMultiplier(Element damageType, Element damageRecieverType, bool justSwitchedIn)
    {
        float output = 0;

        float neutral = 1;
        float superEffective = 2;
        float ineffective = 0.5f;

        if (damageType == Element.Special || damageRecieverType == Element.Special) { return neutral; }
        if (damageType == damageRecieverType) { return neutral; }

        switch (damageType)
        {
            case Element.Fire:
                switch (damageRecieverType)
                {
                    case Element.Water:
                        output = ineffective;
                        break;
                    case Element.Grass:
                        output = superEffective;
                        break;
                }
                break;
            case Element.Water:
                switch (damageRecieverType)
                {
                    case Element.Fire:
                        output = superEffective;
                        break;
                    case Element.Grass:
                        output = ineffective;
                        break;
                }
                break;
            case Element.Grass:
                switch (damageRecieverType)
                {
                    case Element.Fire:
                        output = ineffective;
                        break;
                    case Element.Water:
                        output = superEffective;
                        break;
                }
                break;
            default:
                UnityEngine.Debug.Log("Type Multiplier is missing");
                output = neutral;
                break;
        }


        // Switching into a ineffective move should heal you
        if (justSwitchedIn && output == ineffective)
            output *= -1f;
        
        return output;
    }

    public void TakeDamage(int damageAmount, Element damageType, Monster damageDealer, Monster damageReciever)
    {
        if(!alive) return;
        if (immuneForTurns > 0) return;

        int damageToTake = CalculateDamage(damageAmount, damageType, damageDealer, damageReciever);

        curHealth -= damageToTake;
        curHealth = Math.Clamp(curHealth, 0, damageReciever.maxHealth);

        if (curHealth == 0)
        {

            alive = false;
        }
    }
}
