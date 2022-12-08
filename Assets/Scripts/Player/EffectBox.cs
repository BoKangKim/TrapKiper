using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EffectBox", menuName = "ScriptableObjects/EffectBox",order =1 )]
public class EffectBox : ScriptableObject
{
    [Header("[PLAYER]========")]
    public GameObject jumpEffect = null;

    [Header("[Nomal_Attack]========")]
    public GameObject basicAttackPrefab = null;
    public GameObject basicAttackIndicator = null;

    [Header("[Skill_Effect]========")]
    public GameObject drainEffect = null;
    public GameObject FogEffect   = null;
    public GameObject skillIndicator = null;





}
