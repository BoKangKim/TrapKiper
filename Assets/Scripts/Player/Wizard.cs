using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BasePlayer
{
    public override IEnumerator CAST_STATE()
    {
        Pool.ObjectInstantiate(gainSkills[0].gameObject,transform.position,Quaternion.identity);

        yield return new WaitUntil(() => !castCheck);

        instSkill = true;
        ChageState(STATE.SKILL_STATE);
        yield break;
    }

    public override IEnumerator SKILL_STATE()
    {
        Pool.ObjectInstantiate(gainSkills[1].gameObject, transform.position, Quaternion.identity);
        instSkill = false;

        ChageState(STATE.MOVE_STATE);
        yield break;
    }
}   
