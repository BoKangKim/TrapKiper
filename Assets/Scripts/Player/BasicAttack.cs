using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private float speed;

    SkillManager sm = null;
    Monster targetMoster = null;
    private bool collisonCheck = false;
    private float distance  = 0;
    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        sm = GameManager.Inst.skillManager;
        this.gameObject.transform.SetParent(sm.BOX.transform);
    }

    private void OnEnable()
    {
        if(FindTarget()!=null)
        targetMoster = FindTarget();
    }

    void FixedUpdate()
    {

        if(targetMoster!=null)
        {
            direction = targetMoster.transform.position+Vector3.up - transform.position;
            transform.Translate(direction * Time.fixedDeltaTime * speed,Space.World);

            if(Vector3.Distance(transform.position,targetMoster.transform.position)<0.5f)
            {
                StartCoroutine(PlayEffect());
                targetMoster.PlayLockIn(false);
            }

        }
        else
        {
            transform.Translate(Vector3.forward*Time.fixedDeltaTime*speed*8);
        }
    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name!="Wizard"&&collisonCheck==false)
        {
            collisonCheck = true;
            StartCoroutine(PlayEffect());
            targetMoster.PlayLockIn(false);
        }

    }

    private IEnumerator PlayEffect()
    {
        GameObject effect=
        Pool.ObjectInstantiate(destroyEffect, transform.position, Quaternion.identity);
        effect.transform.SetParent(sm.BOX.transform);
        yield return new WaitForSeconds(0.3f);

        Pool.ObjectDestroy(effect);
        collisonCheck = false;
        Pool.ObjectDestroy(this.gameObject);
    }

    private Monster FindTarget()
    {
        Monster monster = null;

        float dis    = 0;
        float minDis = 0;
        if (GameManager.Inst.spawnMonsterList.Count == 0) return null;

        minDis = Vector3.Distance(GameManager.Inst.player.transform.position, GameManager.Inst.spawnMonsterList[0].transform.position);
        monster = GameManager.Inst.spawnMonsterList[0];

        for (int i = 0; i < GameManager.Inst.spawnMonsterList.Count; i++)
        {
            dis = Vector3.Distance(GameManager.Inst.player.transform.position, GameManager.Inst.spawnMonsterList[i].transform.position);

            if(dis<minDis)
            {
                minDis = dis;
                monster = GameManager.Inst.spawnMonsterList[i];
            }
           
        }
        monster.PlayLockIn();

        return monster;
    }


}
