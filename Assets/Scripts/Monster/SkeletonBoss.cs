using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static BehaviorTree.BehaviorTreeMan;

public class SkeletonBoss : Monster
{

    private void Start()
    {
        agent.isStopped = true;
        AttackArange = 2f;
    }

    protected override void RootNodeInit()
    {
        root = Selector
            (
                 Sequence
                 (
                     IF(NotIf(SpawnMotion).condition).Action(StopMoveAction)
                 ),

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

    private void SetIsSpawnMotionTrue()
    {
        spawnMotion = true;
    }
    private void SetIsAttackFalse()
    {
        isAttack = false;
        a = 0;
    }

    private void SetIsAttackTrue()
    {
        isAttack = true;
    }

    private Func<bool> SpawnMotion
    {
        get
        {
            return () =>
            {
                return spawnMotion;
            };
        }
    }

    private Func<bool> IsArrangeIn
    {
        get
        {
            return () =>
            {
                return Vector3.Distance(player.transform.position, this.gameObject.transform.position) <= AttackArange;
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

    int a = 0;

    private Action Attack
    {
        get
        {
            return () =>
            {
                if (a == 1) return;
                a++;

                if(monsterData.info.curHp <= monsterData.info.maxHp*0.3f)
                {
                    monsterAni.SetTrigger("skillattack");
                    return;
                }

                int count = UnityEngine.Random.Range(0, 2);
                if (count == 1)
                    monsterAni.SetTrigger("powerattack");
                else
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
