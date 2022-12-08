using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRain : ISkill
{
    private List<Monster> hitMonster = new List<Monster>();
    private GameObject skillIndicator = null;
    private GameObject skillCast = null;

    private void OnEnable()
    {
        this.transform.position = player.transform.position + 
            (player.transform.forward.normalized * myData.info.skillRange);

        StartCoroutine(PlaySkill());
    }

    private void FixedUpdate()
    {
        if (skillIndicator.transform.position.y>0)
            skillIndicator.transform.Translate(0, -1.5f * Time.fixedDeltaTime, 0, Space.World);
    }

    private void OnDisable()
    {
        for (int i = 0; i < hitMonster.Count; i++)
        {
            if (hitMonster[i] == null) continue;
            hitMonster[i].PlayLockIn(false);
        }
        hitMonster.Clear();
        myCollider.enabled = false;
    }
    
    protected override IEnumerator PlaySkill()
    {
        //Inst Indicator
        skillIndicator = Pool.ObjectInstantiate(myData.info.addEffect[0], transform.position, Quaternion.identity);
        skillIndicator.transform.SetParent(sm.transform);

        //Position Change
        skillIndicator.transform.position = player.transform.position +
            (player.transform.forward.normalized * myData.info.skillRange) + (Vector3.up * 2);

        //Inst& start CastSkill
        StartCoroutine(CastSkill());

        yield return new WaitUntil(() => player.instSkill);

        //Destroy indicator
        Pool.ObjectDestroy(skillIndicator);

        //Collider On,Off
        StartCoroutine(OnCollider());

        //Skill Particle Play
        effect.Play();
        player.instSkill = false;

        yield return new WaitForSeconds(myData.info.playTime);

        //Skill Particle Stop
        effect.Stop();
        Pool.ObjectDestroy(this.gameObject);
        yield break;
    }

    IEnumerator OnCollider()
    {
        while(true)
        {
            myCollider.enabled = true;
            yield return new WaitForSeconds(1f);
            myCollider.enabled = false;
        }
    }

    protected override IEnumerator CastSkill()
    {
        //Inst CastEffect
        skillCast = Pool.ObjectInstantiate(myData.info.addEffect[1], transform.position + (player.transform.forward.normalized), Quaternion.identity);
        skillCast.transform.SetParent(sm.transform);

        //Position Change
        skillCast.transform.position = player.transform.position +
         (player.transform.forward.normalized);

        yield return new WaitUntil(() => player.castCheck);
        yield return new WaitForSeconds(0.5f);

        skillCast.GetComponent<ParticleSystem>().Play();
        yield return new WaitUntil(() => !player.castCheck);
        skillCast.GetComponent<ParticleSystem>().Stop();

        Pool.ObjectDestroy(skillCast);
        yield break;
    }

    private void OnTriggerEnter(Collider other)
    {
        Monster monster = null;

        if (other.gameObject.TryGetComponent<Monster>(out monster))
        {
            monster.PlayLockIn();
            monster.TransferDamage(1);

            hitMonster.Add(monster);
        }

    }
}
