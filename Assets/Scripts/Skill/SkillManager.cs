using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    SkillData.Info defaultInfo;
    [SerializeField] private List<SkillData> skills = null;
    private GameObject box = null;
    public GameObject BOX { get { return box; } private set { box = value; } }


    private void Awake()
    {
        box = new GameObject("SkillBox");
    }

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

    public SkillData GetSkill(int index)
    {
        if (index > skills.Count - 1) return null;
        return skills[index];
    }


}
