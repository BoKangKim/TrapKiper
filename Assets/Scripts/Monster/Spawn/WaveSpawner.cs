using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    //웨이브 정보를 담은 스크립터블 오브젝트
    [SerializeField] private WaveData waveData;
    [SerializeField] private Transform spawnPoint;


    private GameObject[] waveMonster;
    private GameObject bossMonster;

    private WaitForSeconds spawnInterval;
    private WaitForSeconds waveInterval;

    private void Awake()
    {
        InitData();

        StartCoroutine(WaveStart());
    }

    //데이터 할당
    private void InitData()
    {
        waveMonster = new GameObject[waveData.MonsterPrefabName.Length];

        for (int i = 0; i < waveMonster.Length; i++)
        {
            waveMonster[i] = Resources.Load<GameObject>(waveData.MonsterPrefabName[i]);
        }

        bossMonster = Resources.Load<GameObject>(waveData.BossMonsterPrefabName);

        spawnInterval = new WaitForSeconds(waveData.SpawnInterval);
        waveInterval = new WaitForSeconds(waveData.WaveInterval);
    }

    //웨이브 주기 코루틴
    private IEnumerator WaveStart()
    {
        int monsterKindNum = 0;
        int waveCount = 0;
        int monsterCount = 0;

        while(true)
        {
            waveCount++;

            if (waveMonster.Length > 1 && waveCount <= waveData.MonsterChangeNum[waveData.MonsterChangeNum.Length - 1])
            {
                if (waveCount == waveData.MonsterChangeNum[monsterKindNum])
                    monsterKindNum++;
            }

            while (true)
            {
                if (monsterCount == waveData.MonsterCount)
                {
                    monsterCount = 0;
                    break;
                }

                monsterCount++;

                Pool.ObjectInstantiate(waveMonster[monsterKindNum], spawnPoint.position, Quaternion.identity);

                yield return spawnInterval;
            }

            yield return waveInterval;

            if (waveCount == waveData.MaxWaveCount)
            {
                Pool.ObjectInstantiate(bossMonster, spawnPoint.position, Quaternion.identity);
                yield break;
            }
        }

    }



}
