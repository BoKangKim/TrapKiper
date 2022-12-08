using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private float speed;
    [SerializeField] private float dmamage;

    Monster targetMoster = null;
    private bool collisonCheck = false;
    private Vector3 direction = Vector3.zero;

    private void Awake()
    {
        this.gameObject.transform.SetParent(GameManager.Inst.GetSkillManager.BOX.transform);
    }

    private void OnEnable()
    {
        if ((targetMoster = FindTarget()) == null)
            return;
    }

    private void OnDisable()
    {
        targetMoster = null;
        collisonCheck = false;
        direction = Vector3.zero;
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
                targetMoster.TransferDamage(dmamage);
                targetMoster.PlayLockIn(false);
            }
        }
        else
        {
            transform.Translate(Vector3.forward*Time.fixedDeltaTime*speed*8);
        }
    }

    //find target
    private Monster FindTarget()
    {
        int count = 0;
        int index = 0;
        float dis = 0;
        float minDis = 0;

        Monster monster = null;
        
        if(GameManager.Inst.GetPlayer == null)
        {
            return monster;
        }
        if(GameManager.Inst.TryGetMonster(0,out monster) == false)
        {
            return monster;
        }


        minDis = Vector3.Distance(GameManager.Inst.GetPlayer.transform.position, monster.transform.position);

        while (GameManager.Inst.TryGetMonster(count ,out monster) == true) 
        {
            dis = Vector3.Distance(GameManager.Inst.GetPlayer.transform.position, monster.transform.position);

            if (dis < minDis)
            {
                index = count;
                minDis = dis;
            }

            count++;
        }

        GameManager.Inst.TryGetMonster(index,out monster);

        monster.PlayLockIn();
        return monster;
    }

    //destroy effect
    private IEnumerator PlayEffect()
    {
        GameObject effect =
        Pool.ObjectInstantiate(destroyEffect, transform.position, Quaternion.identity);
        effect.transform.SetParent(GameManager.Inst.GetSkillManager.BOX.transform);

        yield return new WaitForSeconds(0.3f);

        Pool.ObjectDestroy(effect);
        collisonCheck = false;
        Pool.ObjectDestroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Wizard" && collisonCheck == false)
        {
            collisonCheck = true;
            StartCoroutine(PlayEffect());

            if(targetMoster!=null&& (collision.gameObject==targetMoster.gameObject))
                targetMoster.TransferDamage(dmamage);

            if (targetMoster != null)
                targetMoster.PlayLockIn(false);
        }
    }

}
