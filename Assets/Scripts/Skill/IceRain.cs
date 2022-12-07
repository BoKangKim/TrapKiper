using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRain : ISkill
{
    List<Monster> hitMonster = new List<Monster>();

    private void OnEnable()
    {
        StartCoroutine(PlaySkill());
    }
    private void OnDisable()
    {
        Debug.Log("»£√‚");
        for (int i = 0; i < hitMonster.Count; i++)
        {
            hitMonster[i].PlayLockIn(false);
        }
    }

    protected override IEnumerator PlaySkill()
    {
        yield return new WaitUntil(() => player.GetComponent<Wizard>().instSkill);

        particleEffect[0].transform.SetParent(sm.BOX.transform);
        particleEffect[0].transform.position = player.transform.position + (player.transform.forward.normalized * myData.info.skillRange);

        StartCoroutine(OnCollider());

        particleEffect[0].Play();
        yield return new WaitForSeconds(myData.info.playTime);
        particleEffect[0].Stop();
        Pool.ObjectDestroy(this.gameObject);

        yield break;
    }

    IEnumerator OnCollider()
    {
        while(true)
        {
            collider.enabled = true;
            yield return new WaitForSeconds(1f);
            collider.enabled = false;
        }
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
