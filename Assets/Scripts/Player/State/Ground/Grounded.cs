using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;

public class Grounded : BaseState
{
    protected MoveMentSM sm = null;

    public Grounded(StateMachine machine) : base(machine) 
    {
        sm = (MoveMentSM)machine;    
    }

}
