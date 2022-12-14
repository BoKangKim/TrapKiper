using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HFSM;

public class MoveState : Grounded
{
    private float axisX = 0f;
    private float axisZ = 0f;
    private float fixedAxisZ = 0f;
    private float fixedAxisX = 0f;
    private Vector3 lookForward = Vector3.zero;
    private Vector3 lookRight = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;

    public MoveState(StateMachine machine) : base(machine) { }

    public override void Enter()
    {
        sm.animator.SetBool("isMove",true);
    }

    public override void UpdateLogic()
    {
        axisX = Input.GetAxis("Horizontal");
        axisZ = Input.GetAxis("Vertical");

        if (axisX == 0 && axisZ == 0)
        {
            sm.ChangeState(sm.idle);
        }
    }

    public override void UpdatePhysics()
    {
        lookForward = new Vector3(sm.cameraArmTr.transform.forward.x, 0, sm.cameraArmTr.transform.forward.z);
        lookRight = new Vector3(sm.cameraArmTr.transform.right.x, 0, sm.cameraArmTr.transform.right.z);

        fixedAxisX = Mathf.Lerp(fixedAxisX, axisX, Time.fixedDeltaTime * 6);
        fixedAxisZ = Mathf.Lerp(fixedAxisZ, axisZ, Time.fixedDeltaTime * 6);
       
        sm.animator.SetFloat("axisX", fixedAxisX);
        sm.animator.SetFloat("axisZ", fixedAxisZ);

        if ((axisX != 0 || axisZ != 0) && sm.castCheck == false)
        {
            moveDir = lookForward * axisZ + lookRight * axisX;
            sm.playerRb.MovePosition(sm.gameObject.transform.position + moveDir.normalized * sm.playerSpeed * Time.fixedDeltaTime);
        }
        else
        {
            moveDir = lookForward * axisZ + lookRight * axisX;
            sm.playerRb.MovePosition(sm.gameObject.transform.position + moveDir.normalized * (sm.playerSpeed / 10) * Time.fixedDeltaTime);
        }
    }

    public override void Exit()
    {
        sm.animator.SetFloat("axisX", 0f);
        sm.animator.SetFloat("axisZ", 0f);
    }
}
