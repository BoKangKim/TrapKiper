using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    SkillData.Info defaultInfo;
    [SerializeField] private List<SkillData> skills = null;

    public GameObject GetSkill(string skillName)
    {
        for(int i = 0; i < skills.Count; i++)
        {
            if(skills[i].info.FindSkill(skillName) == true)
            {
                GameObject skill = Instantiate(skills[i].gameObject,transform,false);
                return skill;
            }
        }

        return null;
    }

    public int GetSkillCount()
    {
        return skills.Count;
    }
}
