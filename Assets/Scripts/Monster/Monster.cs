using System;
using UnityEngine;
using UnityEngine.AI;
using static BehaviorTree.BehaviorTreeMan;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public abstract class Monster : MonoBehaviour
{
    protected Animator monsterAni = null;
    protected Player player = null;
    protected NavMeshAgent agent = null;
    protected BehaviorTree.INode root = null;

    private void Awake()
    {
        Init();
        RootNodeInit();

        if(root == null)
        {
            Debug.LogError("Root Node Is Null, Define Root Node");
        }
    }

    protected virtual void Init()
    {
        player = FindObjectOfType<Player>();
        monsterAni = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected abstract void RootNodeInit();

}
