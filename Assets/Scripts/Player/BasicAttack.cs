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
            Debug.Log("¿òÁ÷¿©");
            direction = targetMoster.transform.position - GameManager.Inst.player.transform.position;
            transform.Translate(direction * Time.fixedDeltaTime * speed,Space.World);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name!="Wizard"&&collisonCheck==false)
        {
            collisonCheck = true;
            StartCoroutine(PlayEffect());
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
        Monster moster = null;

        float dis    = 0;
        float minDis = 0;
        Debug.Log(GameManager.Inst.spawnMonsterList.Count);
        if (GameManager.Inst.spawnMonsterList.Count == 0) return null;

        minDis = Vector3.Distance(GameManager.Inst.player.transform.position, GameManager.Inst.spawnMonsterList[0].transform.position);
        moster = GameManager.Inst.spawnMonsterList[0];

        for (int i = 0; i < GameManager.Inst.spawnMonsterList.Count; i++)
        {
            dis = Vector3.Distance(GameManager.Inst.player.transform.position, GameManager.Inst.spawnMonsterList[i].transform.position);

            if(dis<minDis)
            {
                minDis = dis;
                moster = GameManager.Inst.spawnMonsterList[i];
            }
           
        }
        
        return moster;
    }


}
