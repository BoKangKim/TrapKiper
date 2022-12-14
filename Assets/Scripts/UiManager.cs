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

    public Slider GetPlayerHpBar { get { return playerHpBar;} private set { } }

    public void Awake()
    {
        
    }

    public void Start()
    {
        InitPlayerHpbar();
    }

    private void InitPlayerHpbar()
    {
        GetPlayerHpBar.maxValue = GameManager.Inst.GetPlayer.GetPlayerData().info.maxHp;
        GetPlayerHpBar.value = GameManager.Inst.GetPlayer.GetPlayerData().info.maxHp;
    }


    public void GetSkill(string skillname)
    {
        string[] str = skillname.Split("(");

        for (int i = 0; i < skillSprite.Length; i++)
        {
            if(str[0]+"_Image"== skillSprite[i].name)
            {
                for (int j = 0; j < skillIndex.Length; j++)
                {
                    if(skillIndex[j].sprite.name=="Empty")
                    {
                        skillIndex[j].sprite = skillSprite[i];

                        return;
                    }
                    else
                    {
                        //키 바꾸는 유아이 
                    }


                }
            }
        }

    }


    public void ChangeImage(string arrow)
    {
        for (int i = 0; i < curSkill.childCount; i++)
        {
            if(curSkill.GetChild(i).gameObject.activeSelf==true)
            {
                if(arrow=="Right")
                {
                    if (i== curSkill.childCount-1)
                        break;
                    else
                    {
                        curSkill.GetChild(i).gameObject.SetActive(false);
                        curSkill.GetChild(i + 1).gameObject.SetActive(true);
                        break;
                    }

                }
                else if(arrow == "Left")
                {
                    if (i ==0)
                        break;
                    else
                    {
                        curSkill.GetChild(i).gameObject.SetActive(false);
                        curSkill.GetChild(i - 1).gameObject.SetActive(true);
                        break;
                    }

                }
               
            }
        }

    }




















}
