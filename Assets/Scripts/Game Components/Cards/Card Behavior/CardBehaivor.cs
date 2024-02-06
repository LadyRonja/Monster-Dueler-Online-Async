using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardBehaivor : MonoBehaviour
{
    public bool userIsTarget = false;

    public virtual void ExecuteBehaivor(Monster user, Monster target, Card card)
    {
        Debug.Log("Behaivor not included");
    }
}
