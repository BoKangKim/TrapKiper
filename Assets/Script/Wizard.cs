using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : BasePlayer
{
  
    public override IEnumerator SKILL_STATE()
    {
        castCheck = false;

        yield return new WaitUntil(()=> !instSkill);
        GetComponent<ISkill>().enabled = false;

        ChageState(STATE.MOVE_STATE);
    }
}   
