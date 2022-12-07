using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static BehaviorTree.BehaviorTreeMan;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent),typeof(MonsterData))]
public abstract class Monster : MonoBehaviour
{
    [SerializeField] protected Canvas lockIn = null;
    [SerializeField] private GameObject lockinBox = null;
    [SerializeField] protected Slider slider = null;

    protected MonsterData monsterData = null;
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

        lockIn.gameObject.transform.LookAt(player.transform.position);
    }

    protected virtual void Init()
    {
        player = FindObjectOfType<Wizard>();
        monsterAni = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        monsterData = GetComponent<MonsterData>();

        slider.maxValue = monsterData.info.maxHp;
        slider.value = monsterData.info.maxHp;
        monsterData.info.curHp = monsterData.info.maxHp;
    }

    protected abstract void RootNodeInit();

    protected virtual void GetDamage()
    {

    }
 
    public void PlayLockIn(bool check = true)
    {
        lockinBox.gameObject.SetActive(check);
    }
  

}
