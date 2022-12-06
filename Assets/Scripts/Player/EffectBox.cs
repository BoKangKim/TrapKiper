using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EffectBox", menuName = "ScriptableObjects/EffectBox",order =1 )]
public class EffectBox : ScriptableObject
{
    [Header("[PLAYER]========")]
    [SerializeField] public GameObject jumpEffect = null;
    [Header("[Nomal_Attack]========")]
    [SerializeField] public GameObject basicAttackPrefab = null;
    [SerializeField] public GameObject basicAttackIndicator = null;

    [Header("[Skill_Effect]========")]
    [SerializeField] public GameObject drainEffect = null;
    [SerializeField] public GameObject FogEffect   = null;
    [SerializeField] public GameObject skillIndicator = null;





}
