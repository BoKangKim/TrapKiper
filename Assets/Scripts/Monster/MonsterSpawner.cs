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
            Monster instMonster = null; 
            GameObject obj = Pool.ObjectInstantiate(monster.gameObject, transform.position, Quaternion.identity);
            if(obj.TryGetComponent<Monster>(out instMonster) == false)
            {
                yield break;
            }

            GameManager.Inst.AddMonster(instMonster);
            if (count == monsterAmount)
                yield break;
        }

    }


}
