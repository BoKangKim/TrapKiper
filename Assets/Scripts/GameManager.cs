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
                instance = GameObject.FindObjectOfType<GameManager>();
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
    [SerializeField] public SkillManager skillManager = null;
    [SerializeField] public BasePlayer player = null;
    

}
