using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EffectContainer", menuName = "ScriptableObjects/PlayerEffect", order =1 )]
public class PlayerEffect : ScriptableObject
{
    [Header("[PLAYER]==============")]
    public GameObject jumpEffect = null;

    [Header("[Nomal_Attack]========")]
    public GameObject basicAttackPrefab = null;
    public GameObject basicAttackIndicator = null;

}
