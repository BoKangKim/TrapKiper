using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private float speed;
    private float dmamage;

    Monster targetMoster = null;
    private bool collisonCheck = false;
    private bool followCheck = true;
    private Vector3 direction = Vector3.zero;

    Vector3 nomalFoward;
    Vector3 nomalTarget;
    float distance;
    double inAngle;

    private void Awake()
    {
        dmamage = GameManager.Inst.GetPlayer.GetPlayerData().info.damage;
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
        followCheck = true;
        direction = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (targetMoster != null&& followCheck)
        {
            distance = Vector3.Distance(transform.position, targetMoster.transform.position);

            if (inAngle > (105*Mathf.Deg2Rad)/2|| GameManager.Inst.GetPlayer.GetPlayerData().info.attackRange < distance)
            {
                followCheck=false;
                transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed * 8);
                return;
            }

            direction = targetMoster.transform.position+Vector3.up - transform.position;
            transform.Translate(direction * Time.fixedDeltaTime * speed,Space.World);

            if(targetMoster==null)
            {
                transform.Translate(Vector3.forward * Time.fixedDeltaTime * speed * 8);
            }

            if(distance < 0.5f)
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

       
        if (GameManager.Inst.GetPlayer == null)
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
        GameManager.Inst.TryGetMonster(index, out monster);

        GetInAngle(monster);

        if (inAngle > (105 * Mathf.Deg2Rad) / 2 ||
                minDis > GameManager.Inst.GetPlayer.GetPlayerData().info.attackRange) return null;

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
            Monster monster = null;
            if(collision.gameObject.TryGetComponent<Monster>(out monster))
                monster.TransferDamage(dmamage);

            if (targetMoster != null)
                targetMoster.PlayLockIn(false);
        }
    }

    private void GetInAngle(Monster targetMonster)
    {
        nomalFoward = GameManager.Inst.GetPlayer.transform.forward.normalized;
        nomalTarget = (targetMonster.transform.position - GameManager.Inst.GetPlayer.transform.position).normalized;

        inAngle = Mathf.Acos(Vector3.Dot(nomalFoward, nomalTarget));
    }







}
