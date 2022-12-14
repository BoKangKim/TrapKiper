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
    [SerializeField] private TrapManager tm = null;
    [SerializeField] private UiManager uiManager = null;

    private List<Monster> spawnMonsterList = null;
    private Monster targetMonster = null;

    public int GetMonsterCount { get { return spawnMonsterList.Count; }  private set { } }
    public BasePlayer GetPlayer { get { return player; } private set { } }
    public SkillManager GetSkillManager { get { return skillManager; } private set{} }
    public TrapManager GetTrapManager { get { return tm; } private set { } }
    public UiManager GetUiManager { get { return uiManager; } private set { } }

    private void Awake()
    {
        spawnMonsterList = new List<Monster>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetUiManager.ChangeImage("Left");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GetUiManager.ChangeImage("Right");
        }

    }




    public bool NullCheck<T>(ref T obj) where T : Component
    {
        if(obj != null)
        {
            return false;
        }
        else
        {
            obj = new GameObject(typeof(T).Name).AddComponent<T>();
            return true;
        }
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
            monster = null;
            return false;
        }
    }

    #endregion

}
