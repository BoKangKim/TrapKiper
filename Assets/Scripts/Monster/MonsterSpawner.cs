using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster monster = null;
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
            yield return new WaitForSeconds(3f);
            count++;
            Monster monster = null; 
            GameObject obj = Pool.ObjectInstantiate(monster.gameObject, transform.position, Quaternion.identity);
            if(obj.TryGetComponent<Monster>(out monster) == false)
            {
                monster.gameObject.name = count.ToString();
                GameManager.Inst.spawnMonsterList.Add(monster);
                yield break;
            }

            if (count == monsterAmount)
                yield break;
        }

    }


}
