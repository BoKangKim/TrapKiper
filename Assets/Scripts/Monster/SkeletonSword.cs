using System;
using UnityEngine;
using BehaviorTree;
using static BehaviorTree.BehaviorTreeMan;
using System.Collections;

public class SkeletonSword : Monster
{
    private void Start()
    {
        agent.isStopped = true;
        AttackArange = 1f;
    }

    protected override void RootNodeInit()
    {
        root = Selector
            (
                Sequence
                (
                    NotIf(IsArrangeIn),
                    ActionN(SetDestination),
                    IF(NotIf(GetWalkAniBool).condition).Action(MoveAction)
                ),

                Sequence
                (
                    IF(IsArrangeIn),
                    ActionN(StopMoveAction),
                    IF(NotIf(IsAttack).condition).Action(Attack)
                )
            );
    }

    private void SetIsAttackFalse()
    {
        isAttack = false;
    }

    private void SetIsAttackTrue()
    {
        isAttack = true;
    }
   

    private Func<bool> IsArrangeIn
    {
        get
        {
            return () =>
            {
                return Vector3.Distance(player.transform.position,this.gameObject.transform.position) <= AttackArange;
            };
        }
    }

    private Func<bool> IsStop 
    {
        get
        {
            return () =>
            {
                return agent.isStopped;
            };
        }
    }

    private Action SetDestination
    {
        get
        {
            return () =>
            {
                agent.destination = player.transform.position;
            };
        }
    }

    private Func<bool> GetWalkAniBool 
    { 
        get 
        {
            return () =>
            {
                return monsterAni.GetBool("walk");
            };
        } 
    }

    private Action MoveAction 
    {
        get
        {
            return () =>
            {
                agent.isStopped = false;
                monsterAni.SetBool("walk", true);
                agent.destination = player.transform.position;
            };
        }
    }

    private Action StopMoveAction
    {
        get
        {
            return () =>
            {
                agent.isStopped = true;
                monsterAni.SetBool("walk", false);
            };
        }
    }

    private Action Attack 
    {
        get
        {
            return () =>
             {
                 monsterAni.SetTrigger("attack");
             };
        }
    }

    private Func<bool> IsAttack 
    { 
        get 
        {
            return () =>
            {
                return isAttack;
            };
        } 
    }



}
