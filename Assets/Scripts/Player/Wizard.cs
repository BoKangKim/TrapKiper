using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BasePlayer
{
    public override IEnumerator CAST_STATE()
    {
        Debug.Log(skillIndex);
        InstSkill(skillIndex);

        StartCoroutine(ChangeIdle());
        yield return new WaitUntil(() => !castCheck);

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

        yield return new WaitUntil(()=> !instSkill);

        ChageState(STATE.MOVE_STATE);
        yield break;
    }
}   
