using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallenSword : ISkill
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
        skillIndicator.transform.localScale= new Vector3(0.5f,0.5f,0.5f);

        //Position Change
        IndicatorRotation(skillIndicator.transform);

        //Inst& start CastSkill
        StartCoroutine(CastSkill());

        yield return new WaitUntil(() => player.instSkill);

        //Destroy indicator
        Pool.ObjectDestroy(skillIndicator);

        //Collider On,Off
        StartCoroutine(OnCollider());

        //Skill Particle Play
        effect.Play();
        myData.info.sound.Play();
        player.instSkill = false;

        yield return new WaitForSeconds(myData.info.playTime);

        //Skill Particle Stop
        effect.Stop();
        Pool.ObjectDestroy(this.gameObject);
        yield break;
    }

    IEnumerator OnCollider()
    {
        myCollider.enabled = true;
        yield return new WaitForSeconds(0.1f);
        myCollider.enabled = false;
    }

    protected override IEnumerator CastSkill()
    {
        //Inst CastEffect
        skillCast = Pool.ObjectInstantiate(myData.info.addEffect[1], transform.position+(Vector3.up*0.5f), Quaternion.identity);
        skillCast.transform.SetParent(sm.transform);

        yield return new WaitUntil(() => player.castCheck);
        yield return new WaitForSeconds(0.5f);

        skillCast.GetComponent<ParticleSystem>().Play();
        yield return new WaitUntil(() => !player.castCheck);
        skillCast.GetComponent<ParticleSystem>().Stop();

        Pool.ObjectDestroy(skillCast);
        yield break;
    }   

    private void IndicatorRotation(Transform targetTr)
    {
        RaycastHit hitinfo;
        Vector3 startVec = GameManager.Inst.GetPlayer.transform.position + (Vector3.up * 1.5f) + 
            (GameManager.Inst.GetPlayer.transform.forward * myData.info.skillRange);
        Vector3 y = new Vector3(0, -5f, 0f);
        Vector3 dirVec = ((startVec + y) - startVec).normalized;

        Debug.DrawRay(startVec, dirVec * 5, Color.red);

        if (Physics.Raycast(startVec, dirVec, out hitinfo, Mathf.Infinity))
        {
            targetTr.position = hitinfo.point + (Vector3.up * 0.4f);
            targetTr.rotation = Quaternion.LookRotation(hitinfo.normal);
            targetTr.Rotate(-90, 0, 0);

            this.transform.rotation= Quaternion.LookRotation(hitinfo.normal);
            this.transform.Rotate(-90, 0, 0);
            this.transform.position += Vector3.up;

        }
    }



    private void OnTriggerEnter(Collider other)
    {
        Monster monster = null;

        if (other.gameObject.TryGetComponent<Monster>(out monster))
        {
            monster.PlayLockIn();
            monster.TransferDamage(myData.info.damage);

            hitMonster.Add(monster);
        }

    }
}
