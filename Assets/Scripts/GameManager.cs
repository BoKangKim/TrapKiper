using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance = null;
    public static GameManager Inst
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    instance = new GameObject("GameManager", typeof(GameManager)).GetComponent<GameManager>();
                    if (instance == null)
                        Debug.LogError("##[Error] GameManager Singleton is failed");
                }
            }

            return instance;
        }
    }
    #endregion // Singleton
    [SerializeField] private SkillManager skillManager = null;
    [SerializeField] private BasePlayer player = null;

    private List<Monster> spawnMonsterList = null;
    private Monster targetMonster = null;

    public int GetMonsterCount { get { return spawnMonsterList.Count; }  private set { } }
    public BasePlayer GetPlayer { get { return player; } private set { } }
    public SkillManager GetSkillManager { get { return skillManager; } private set{} }

    private void Awake()
    {
        spawnMonsterList = new List<Monster>();
    }

    private void Update()
    {
        if(targetMonster!=null)
        Debug.Log(targetMonster.name);  
    }

    #region Monster List Control
    public void RemoveMonster(Monster monster)
    {
        spawnMonsterList.Remove(monster);

    }

    public void AddMonster(Monster monster)
    {
        spawnMonsterList.Add(monster);
    }

    public bool TryGetMonster(int index, out Monster monster)
    {
        if(spawnMonsterList.Count > index)
        {
            monster = spawnMonsterList[index];
            return true;
        }
        else
        {
            monster = spawnMonsterList[spawnMonsterList.Count - 1];
            return false;
        }
    }

    #endregion

}
