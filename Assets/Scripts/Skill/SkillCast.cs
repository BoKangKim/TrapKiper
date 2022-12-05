using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCast : ISkill
{
    private void OnEnable()
    {
        StartCoroutine(PlaySkill());
    }

    private void Update()
    {
        if (particleEffect[0].gameObject != null)
            particleEffect[0].gameObject.transform.position = player.transform.position + 
                (player.transform.forward.normalized * myData.info.skillRange)-Vector3.up;
    }

    protected override IEnumerator PlaySkill()
    {
        yield return new WaitUntil(() => player.GetComponent<Wizard>().castCheck);
        yield return new WaitForSeconds(0.5f);

        particleEffect[0].transform.SetParent(sm.BOX.transform);
        particleEffect[0].transform.position = player.transform.position + (player.transform.forward.normalized * myData.info.skillRange);  

        particleEffect[0].Play();
        yield return new WaitForSeconds(myData.info.playTime);
        particleEffect[0].Stop();
        Pool.ObjectDestroy(this.gameObject);
        yield break;
    }

}
