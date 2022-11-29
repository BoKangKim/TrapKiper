using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnJumpEvent();
public delegate void OnJumpEndEvent();
public delegate void OnAttackEvent();
public delegate void OnSkillEvent();

public class AnimationEventReciver : MonoBehaviour
{
    public OnJumpEvent callJumpEvent = null;
    public OnJumpEndEvent callJunpEndEvent=null;
    public OnAttackEvent callAttackEvent = null;
    public OnSkillEvent callSkillEvent = null;

    public void JumpStartEvent()
    {
        if(callJumpEvent!=null)
        {
            callJumpEvent();
        }
    }
    public void JumpEndEvent()
    {
        if (callJumpEvent != null)
        {
            callJunpEndEvent();
        }
    }

    public void AttackEvent()
    {
        if (callAttackEvent != null)
        {
            callAttackEvent();
        }
    }

    public void SkillEvent()
    {
        if (callSkillEvent != null)
        {
            callSkillEvent();
        }
    }

}
