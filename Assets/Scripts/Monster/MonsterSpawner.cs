using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster monster = null;
    [SerializeField] private Monster bossMonster = null;
    [SerializeField] private float spawnTime = 0;
    [SerializeField] private int monsterAmount = 0;

    private void Start()
    {
        StartCoroutine(GenerateMonster());
    }


    IEnumerator GenerateMonster()
    {
        int count=0;
        while(true)
        {
            yield return new WaitForSeconds(spawnTime);
            count++;
            Monster instMonster = null; 
            GameObject obj = Pool.ObjectInstantiate(monster.gameObject, transform.position, Quaternion.identity);
            if(obj.TryGetComponent<Monster>(out instMonster) == false)
            {
                yield break;
            }

            GameManager.Inst.AddMonster(instMonster);

            if (count == monsterAmount)
            {
                yield return new WaitForSeconds(spawnTime);

                GameObject boss = Pool.ObjectInstantiate(bossMonster.gameObject, transform.position, bossMonster.gameObject.transform.rotation = Quaternion.Euler(0,90,0));
                if (boss.TryGetComponent<Monster>(out instMonster) == false)
                {
                    yield break;
                }
                GameManager.Inst.AddMonster(instMonster);

                yield break;
            }
        }

    }


}
