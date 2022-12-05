using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRain : ISkill
{
    private void OnEnable()
    {
        StartCoroutine(PlaySkill());
    }

    protected override IEnumerator PlaySkill()
    {
        yield return new WaitUntil(() => player.GetComponent<Wizard>().instSkill);

        particleEffect[0].transform.SetParent(sm.BOX.transform);
        particleEffect[0].transform.position = player.transform.position + (player.transform.forward.normalized * myData.info.skillRange);

        particleEffect[0].Play();
        yield return new WaitForSeconds(myData.info.playTime);
        particleEffect[0].Stop();
        Pool.ObjectDestroy(this.gameObject);

        yield break;
    }


}
