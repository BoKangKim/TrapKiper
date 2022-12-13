using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
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
    protected bool spawnMotion = false;
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

    private void OnEnable()
    {
        slider.maxValue = monsterData.info.maxHp;
        slider.value = monsterData.info.maxHp;
        monsterData.info.curHp = monsterData.info.maxHp;
    }
    private void Update()
    {
        root.Run();

        lockIn.gameObject.transform.LookAt(Camera.main.transform.position);
    }

    protected virtual void Init()
    {
        player = FindObjectOfType<Wizard>();
        monsterAni = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        monsterData = GetComponent<MonsterData>();
    }

    


    protected abstract void RootNodeInit();

    public void PlayLockIn(bool check = true)
    {
        lockinBox.gameObject.SetActive(check);  
    }

    Coroutine curCorutine;

    public void TransferDamage(float damage)
    {
        if (curCorutine != null)
        {
            monsterData.info.curHp = damageHp;
            slider.value = monsterData.info.curHp;
            StopCoroutine(curCorutine);
        }

        curCorutine = StartCoroutine(ChangeHp(damage));
    }
    float damageHp;

    IEnumerator ChangeHp(float damage)
    {
        damageHp = monsterData.info.curHp - damage;

        while (true)
        {
            monsterData.info.curHp = Mathf.Lerp(monsterData.info.curHp, damageHp, Time.deltaTime*3);
            slider.value = monsterData.info.curHp;

            if (monsterData.info.curHp <= damageHp+0.1f)
            {
                monsterData.info.curHp = damageHp;

                if (monsterData.info.curHp <= 0)
                {
                    GameManager.Inst.RemoveMonster(this);
                    int RandomCount = UnityEngine.Random.Range(0, 3);
                    //if (RandomCount == 2)
                    Pool.ObjectInstantiate(monsterData.info.randomSkill, transform.position + Vector3.up, Quaternion.identity);

                    Pool.ObjectDestroy(this.gameObject);
                    yield break;
                }
                else
                {
                    yield break;
                }
            }

            yield return null;
        }
    }

}
