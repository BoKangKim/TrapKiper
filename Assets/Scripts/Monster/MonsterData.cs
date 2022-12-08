using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterData : MonoBehaviour
{
    [System.Serializable]
    public struct Info
    {
        public string name;
        public float maxHp;
        public float damage;
        public float curHp;
        public GameObject randomSkill;
    }
    [SerializeField] public Info info;
}
