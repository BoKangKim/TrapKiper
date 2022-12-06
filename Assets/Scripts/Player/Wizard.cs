using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BasePlayer
{
    public override IEnumerator CAST_STATE()
    {
        
        Pool.ObjectInstantiate(gainSkills[0].gameObject,transform.position,Quaternion.identity);
        GameObject sillIndicator= Pool.ObjectInstantiate(myEffectBox.skillIndicator, transform.position, Quaternion.identity);
        sillIndicator.transform.position = transform.position + (transform.forward.normalized * 5);
        //GameObject drainEffect = Pool.ObjectInstantiate(myEffectBox.drainEffect, transform.position, Quaternion.identity);
        //drainEffect.transform.position = transform.position + (transform.up.normalized);

        StartCoroutine(ChangeIdle());
        yield return new WaitUntil(() => !castCheck);
        Pool.ObjectDestroy(sillIndicator);
        //Pool.ObjectDestroy(drainEffect);

        ChageState(STATE.SKILL_STATE);
        yield break;
    }

    IEnumerator ChangeIdle()
    {
        while(castCheck)
        {
            fixedAxisZ = Mathf.Lerp(fixedAxisZ, 0, 0.1f);
            playerAnimator.SetFloat("axisZ", fixedAxisZ);
            fixedAxisX = Mathf.Lerp(fixedAxisX, 0, 0.1f);
            playerAnimator.SetFloat("axisX", fixedAxisX);

            playerSpeed = 0;

            yield return null;
        }
    }




    public override IEnumerator SKILL_STATE()
    {
        instSkill = true;
        Pool.ObjectInstantiate(gainSkills[1].gameObject, transform.position, Quaternion.identity);
        instSkill = false;

        ChageState(STATE.MOVE_STATE);
        yield break;
    }
}   
