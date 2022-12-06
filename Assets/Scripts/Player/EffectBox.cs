using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EffectBox", menuName = "ScriptableObjects/EffectBox",order =1 )]
public class EffectBox : ScriptableObject
{
    [SerializeField] public GameObject basicAttackPrefab = null;
    [SerializeField] public GameObject basicAttackIndicator = null;
    [SerializeField] public GameObject jumpEffect = null;
}
