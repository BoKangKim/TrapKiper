using System;
using UnityEngine;
using UnityEngine.AI;
using static BehaviorTree.BehaviorTreeMan;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public abstract class Monster : MonoBehaviour
{
    protected Animator monsterAni = null;
    protected Wizard player = null;
    protected NavMeshAgent agent = null;
    protected BehaviorTree.INode root = null;
    protected bool isAttack = false;
    public float AttackArange { get; protected set; }

    private void Awake()
    {
        Init();
        RootNodeInit();

        if(root == null)
        {
            Debug.LogError("Root Node Is Null, Define Root Node");
        }
    }
    

    private void Update()
    {
        root.Run();
    }

    protected virtual void Init()
    {
        player = FindObjectOfType<Wizard>();
        monsterAni = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    protected abstract void RootNodeInit();

}
