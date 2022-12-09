using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObject : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Inst.GetPlayer.gameObject)
        {
            GameManager.Inst.GetPlayer.AddGainSkillList(GameManager.Inst.GetSkillManager.GetSkill(0));

            Pool.ObjectDestroy(this.gameObject);
        }
    }   
   
}
