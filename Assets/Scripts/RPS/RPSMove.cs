using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RPS { UNSELECTED, ROCK, PAPER, SCISSOR}

[Serializable]
public class RPSMove
{
    public RPS selectedMove;
}
