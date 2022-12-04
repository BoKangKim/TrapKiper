using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillData))]
public abstract class ISkill : MonoBehaviour
{
    protected SkillData myData = null;

    private void Awake()
    {
        myData = GetComponent<SkillData>();
        myData.info.player = GetComponent<BasePlayer>();
    }
      
    public void ActiveSkill()
    {
        StartCoroutine(PlaySkill());
    }

    public void StopSkill()
    {
        StopCoroutine(PlaySkill());
    }

    protected abstract IEnumerator PlaySkill(); 
}
