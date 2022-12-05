using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    [SerializeField] private GameObject destroyEffect = null;
    [SerializeField] private float speed;

    SkillManager sm = null;

    private void Awake()
    {
        sm = GameManager.Inst.skillManager;
        this.gameObject.transform.SetParent(sm.BOX.transform);
    }

    void FixedUpdate()
    {
        transform.Translate(0,0,Time.fixedDeltaTime*speed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name!="Wizard")
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        GameObject effect=
        Pool.ObjectInstantiate(destroyEffect, transform.position, Quaternion.identity);
        effect.transform.SetParent(sm.BOX.transform);

        yield return new WaitForSeconds(0.5f);

        Pool.ObjectDestroy(effect); 
        Pool.ObjectDestroy(this.gameObject);
    }

}
