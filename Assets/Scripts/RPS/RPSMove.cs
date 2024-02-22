using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RPS { UNSELECTED, SOLDIER, POLITICIAN, ASSASSIN, COUNTER_SPELL, FIREBALL}

[Serializable]
public class RPSMove
{
    public RPS selectedMove;
}
