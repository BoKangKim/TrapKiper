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
       
        effect[0] = Instantiate(myData.info.effect[0], 
            transform.position, Quaternion.identity,this.transform);
        effect[0].transform.localPosition += new Vector3(0, 0, 5);
        
        effect[0].GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(3f);
        effect[0].GetComponent<ParticleSystem>().Stop();

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
