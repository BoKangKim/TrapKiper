using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image[] skills = null;
    [SerializeField] private Transform curSkill = null; 
    [SerializeField] private Slider playerHpBar = null;

    private GameObject curSkillImage = null;

    public Slider GetPlayerHpBar { get { return playerHpBar;} private set { } }

    public void Awake()
    {
        instImge();

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


    private void instImge()
    {
        GameObject Image = null;
        for (int i = 0; i < skills.Length; i++)
        {
            Image = Pool.ObjectInstantiate(skills[i].gameObject,Vector3.zero,Quaternion.identity);
            Image.transform.SetParent(curSkill);
            Image.transform.localPosition = Vector3.zero;
            Image.SetActive(false);
        }
    }


    public void ChangeImage(string skillname)
    {
        if (curSkillImage == null)
            curSkillImage = curSkill.GetChild(0).gameObject;

        curSkillImage.gameObject.SetActive(false);

        string[] str = skillname.Split("(");
        string[] name = null;

        for (int i = 1; i < curSkill.childCount; i++)
        {
            name = curSkill.GetChild(i).name.Split("(");

            if (name[0] == str[0]+ "_Image")
            {
                curSkill.GetChild(i).gameObject.SetActive(true);
                curSkillImage = curSkill.GetChild(i).gameObject;
                break;
            }
        }

    }

}
