using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [System.Serializable]
    public struct Info
    {
        public float maxHp;
        public float damage;
        public float attackRange;
        public float curHp;
    }
    [SerializeField] public Info info;
}
