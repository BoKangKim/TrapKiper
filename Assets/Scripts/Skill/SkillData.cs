using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData : MonoBehaviour
{
    [System.Serializable]
    public struct Info
    {
        public string name;
        public GameObject[] effect;
        public float playTime;
        public float skillRange;
        public AudioClip sound;
        [HideInInspector] public bool isPlaying;

        public bool FindSkill(string name)
        {
            if (name.Equals(this.name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [SerializeField] public Info info;
}
