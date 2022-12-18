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
        if(GetUiManager.changeKeyCheck)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
               
                GetPlayer.ReplaceSkill(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex),0);
                GetUiManager.ReplaceSkillImage(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex).name, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GetPlayer.ReplaceSkill(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex), 1);
                GetUiManager.ReplaceSkillImage(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex).name, 1);

            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                GetPlayer.ReplaceSkill(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex), 2);
                GetUiManager.ReplaceSkillImage(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex).name, 2);

            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                GetPlayer.ReplaceSkill(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex), 3);
                GetUiManager.ReplaceSkillImage(GetSkillManager.GetSkill(GetPlayer.gainSkillIndex).name, 3);

            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)&& GetPlayer.Check(0))
            {
                GetPlayer.skillIndex = 0;
                GetPlayer.CallSkill(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2)&& GetPlayer.Check(1))
            {
                GetPlayer.skillIndex = 1;
                GetPlayer.CallSkill(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3)&& GetPlayer.Check(2))
            {
                GetPlayer.skillIndex = 2;
                GetPlayer.CallSkill(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4)&& GetPlayer.Check(3))
            {
                GetPlayer.skillIndex = 3;
                GetPlayer.CallSkill(4);
            }
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
