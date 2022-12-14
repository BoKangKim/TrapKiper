using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetObject : MonoBehaviour
{
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == GameManager.Inst.GetPlayer.gameObject)
        {
            int random = Random.Range(0, GameManager.Inst.GetSkillManager.GetSkillCount());

            Debug.Log(GameManager.Inst.GetSkillManager.GetSkill(random).name);

            GameManager.Inst.GetUiManager.GetSkill(GameManager.Inst.GetSkillManager.GetSkill(random).name);

            GameManager.Inst.GetPlayer.AddGainSkillList(GameManager.Inst.GetSkillManager.GetSkill(random));

            Pool.ObjectDestroy(this.gameObject);
        }
    }   
   
}
