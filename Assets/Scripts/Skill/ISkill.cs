using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillData))]
public abstract class ISkill : MonoBehaviour
{
    protected SkillData myData    = null;
    protected SkillManager sm     = null;
    protected BasePlayer player   = null;
    protected ParticleSystem[] particleEffect = null;

    private void Awake()
    {
        myData = GetComponent<SkillData>();
        player = FindObjectOfType<BasePlayer>();

        particleEffect = new ParticleSystem[myData.info.effect.Length];

        for (int i = 0; i < myData.info.effect.Length; i++)
        {
            particleEffect[i] = myData.info.effect[i].GetComponent<ParticleSystem>();
        }


        if (sm == null)
        {
            sm = FindObjectOfType<SkillManager>();

            if(sm == null)
            {
                Debug.LogError("Not Found Skill Manager");
                return;
            }
        }
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
