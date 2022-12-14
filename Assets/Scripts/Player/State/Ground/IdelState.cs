using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;

public class IdelState : Grounded
{
    float axisX = 0f;
    float axisZ = 0f;

    public IdelState(StateMachine machine) : base(machine) { }

    public override void Enter()
    {
        axisX = 0f;
        axisZ = 0f;
        sm.animator.SetBool("isMove",false);
    }

    public override void UpdateLogic()
    {
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        if(axisX != 0 || axisZ != 0)
        {
            sm.ChangeState(sm.move);
        }
    }
}
