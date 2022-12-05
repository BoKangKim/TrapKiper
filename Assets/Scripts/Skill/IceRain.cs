using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRain : ISkill
{
    GameObject[] effect;

    private void OnEnable()
    {
        effect = new GameObject[myData.info.effect.Length];

        StartCoroutine(PlaySkill());
    }

    private void Update()
    {
        
    }

    protected override IEnumerator PlaySkill()
    {
        yield return StartCoroutine(PlayCast());
       
        effect[0] = Pool.ObjectInstantiate(myData.info.effect[0], 
            transform.position, Quaternion.identity);
        effect[0].transform.SetParent(sm.BOX.transform);

        effect[0].transform.position =  myData.info.player.transform.position + (myData.info.player.transform.forward.normalized * 5f);
        
        effect[0].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3f);
        effect[0].GetComponent<ParticleSystem>().Stop();
        Pool.ObjectDestroy(effect[0]);

        myData.info.player.instSkill = false;
        yield break;
    }

    IEnumerator PlayCast()
    {
        effect[1] = Instantiate(myData.info.effect[1], myData.info.player.attackStartZone.position, transform.rotation,this.transform);
        effect[1].transform.localPosition += new Vector3(0, -1.3f, 0.8f);
        effect[1].GetComponent<ParticleSystem>().Play();
        yield return new WaitUntil(() => !myData.info.player.GetComponent<Wizard>().castCheck);
        effect[1].GetComponent<ParticleSystem>().Stop();
        yield break;
    }


}
