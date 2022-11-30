using System;
using UnityEngine;
using static BehaviorTree.BehaviorTreeMan;


public class SkeletonSword : Monster
{
    private Func<bool> IsArrangeIn
    {
        get
        {
            return () =>
            {

            };
        }
    }

    private Action SetDestinationPlayer
    {
        get
        {
            return () =>
            {

            };
        }
    }

    private Action MoveAction
    {
        get
        {
            return () =>
            {
                monsterAni.SetBool("walk", true);
            };
        }
    }

    private Action StopMoveAction
    {
        get
        {
            return () =>
            {
                monsterAni.SetBool("walk", false);
            };
        }
    }

    protected override void RootNodeInit()
    {
        Debug.Log("");
    }
}
