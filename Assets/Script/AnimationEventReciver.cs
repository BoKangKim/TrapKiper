using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void OnJumpEvent();
public delegate void OnJumpEndEvent();
public delegate void OnAttackEvent();
public delegate void OnSkillEndEvent();
public delegate void OnskillStartEvent();

public class AnimationEventReciver : MonoBehaviour
{
    public OnJumpEvent callJumpEvent = null;
    public OnJumpEndEvent callJunpEndEvent=null;
    public OnAttackEvent callAttackEvent = null;
    public OnSkillEndEvent callSkillEndEvent = null;
    public OnskillStartEvent callSkillStartEvent = null;


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
    public void SkillEndEvent()
    {
        if (callSkillEndEvent != null)
        {
            callSkillEndEvent();
        }
    }
    public void SkillStartEvent()
    {
        if (callSkillStartEvent != null)
        {
            callSkillStartEvent();
        }
    }
}
