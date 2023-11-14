using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="WaveData",menuName = "ScriptableObject/WaveInfo",order = 0)]
public class WaveData : ScriptableObject
{
    //������ ���� ������ �̸���
    [SerializeField]
    private string[] monsterPrefabName;
    public string[] MonsterPrefabName { get { return monsterPrefabName; } }

    //������ �������� ������ �̸�
    [SerializeField]
    private string bossMonsterPrefabName;
    public string BossMonsterPrefabName { get { return bossMonsterPrefabName; } }

    //���̺� ��
    [SerializeField]
    private int maxWaveCount;
    public int MaxWaveCount { get { return maxWaveCount; } }

    //1���̺� �� ������ ���� �� 
    [SerializeField]
    private int monsterCount;
    public int MonsterCount { get { return monsterCount; } }

    //���̺� �ֱ�
    [SerializeField]
    private float waveInterval;
    public float WaveInterval { get { return waveInterval;} }

    //���� �ֱ�
    [SerializeField]
    private float spawnInterval;
    public float SpawnInterval { get { return spawnInterval; } }

    //�ٸ� ���� ���� ���̺� Ÿ�̹�
    [SerializeField]
    private int[] monsterChangeNum;
    public int[] MonsterChangeNum { get { return monsterChangeNum; } }


}
