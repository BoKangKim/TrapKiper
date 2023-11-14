using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveData",menuName = "ScriptableObject/WaveInfo",order = 0)]
public class WaveData : ScriptableObject
{
    //생성될 몬스터 프리팹 이름들
    [SerializeField]
    private string[] monsterPrefabName;
    public string[] MonsterPrefabName { get { return monsterPrefabName; } }

    //생성될 보스몬스터 프리팹 이름
    [SerializeField]
    private string bossMonsterPrefabName;
    public string BossMonsterPrefabName { get { return bossMonsterPrefabName; } }

    //웨이브 수
    [SerializeField]
    private int maxWaveCount;
    public int MaxWaveCount { get { return maxWaveCount; } }

    //1웨이브 당 생성될 몬스터 수 
    [SerializeField]
    private int monsterCount;
    public int MonsterCount { get { return monsterCount; } }

    //웨이브 주기
    [SerializeField]
    private float waveInterval;
    public float WaveInterval { get { return waveInterval;} }

    //생성 주기
    [SerializeField]
    private float spawnInterval;
    public float SpawnInterval { get { return spawnInterval; } }

    //다른 몬스터 생성 웨이브 타이밍
    [SerializeField]
    private int[] monsterChangeNum;
    public int[] MonsterChangeNum { get { return monsterChangeNum; } }


}
