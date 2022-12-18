using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image[] skillIndex = null;
    [SerializeField] private Sprite[] skillSprite = null; 
    [SerializeField] private Transform curSkill = null; 
    [SerializeField] private Slider playerHpBar = null;
    [SerializeField] private GameObject selectBox = null;
    [SerializeField] private Image getSkillImage = null;

    [HideInInspector] public bool changeKeyCheck = false;

    private Sprite getSkillSprite = null;

    public Slider GetPlayerHpBar { get { return playerHpBar;} private set { } }

    public void Start()
    {
        InitPlayerHpbar();
    }

    private void InitPlayerHpbar()
    {
        GetPlayerHpBar.maxValue = GameManager.Inst.GetPlayer.GetPlayerData().info.maxHp;
        GetPlayerHpBar.value = GameManager.Inst.GetPlayer.GetPlayerData().info.maxHp;
    }


    public void GetSkillImage(string skillname)
    {
        for (int i = 0; i < skillSprite.Length; i++)
        {
            if(skillSprite[i].name == skillname + "_Image")
                getSkillSprite = skillSprite[i];
        }

        for (int i = 0; i < skillIndex.Length; i++)
        {
            if (skillIndex[i].sprite.name == "Empty")
            {
                if (i == skillIndex.Length - 1)
                {
                    getSkillImage.sprite = getSkillSprite;
                    changeKeyCheck = true;
                    selectBox.SetActive(true);
                    return;
                }
            }
            else
                break;
        }

        for (int i = 0; i < skillIndex.Length; i++)
        {
            if (skillIndex[i].sprite.name == getSkillSprite.name)
            {
                //레벨업 유아이
                Debug.Log("레벨업 유아이 출력");
                return;
            }
            else if (skillIndex[i].sprite.name != getSkillSprite.name && skillIndex[i].sprite.name == "Empty")
            {
                getSkillImage.sprite = getSkillSprite;
                changeKeyCheck = true;
                selectBox.SetActive(true);
                return;
            }
        }
    }

    public void ReplaceSkillImage(string skillname, int index)
    {
        for (int i = 0; i < skillSprite.Length; i++)
        {
            if (skillSprite[i].name== skillname+"_Image")
            {
                skillIndex[index].sprite = skillSprite[i];
                changeKeyCheck = false;
                selectBox.SetActive(false);
                return;
            }
        }

    }


}
