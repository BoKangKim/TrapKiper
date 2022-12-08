using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkillData))]
public abstract class ISkill : MonoBehaviour
{
    protected SkillData myData    = null;
    protected SkillManager sm     = null;
    protected BasePlayer player   = null;
    protected Collider myCollider   = null;
    protected ParticleSystem effect = null;

    private void Awake()
    {
        myData = GetComponent<SkillData>();
        effect = this.gameObject.GetComponent<ParticleSystem>();
        player = FindObjectOfType<BasePlayer>();
        myCollider = GetComponent<Collider>();

        if (sm == null)
        {
            sm = FindObjectOfType<SkillManager>();

            if(sm == null)
            {
                Debug.LogError("Not Found Skill Manager");
                return;
            }
        }
        this.gameObject.transform.SetParent(sm.transform);
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

    protected abstract IEnumerator CastSkill();

}
