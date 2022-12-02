using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Character Settings", menuName = "Orbital Aiming System/Character Profile", order = 1)]
public class OrbitalCharacterProfile : ScriptableObject
{
    [Serializable]
    public class HandIkSettings
    {
        [Serializable]
        public class HandSetting
        {
            [Tooltip("Positional hand offset for the IK calculation. This value is added to the respective weapon profile.")]
            public Vector3 positionOffset = new Vector3();
            [Tooltip("Rotational hand offset for the IK calculation. This value is added to the respective weapon profile.")]
            public Vector3 rotationOffset = new Vector3();
        }
        [Tooltip("Left hand IK settings")]
        public HandSetting leftHand;
        [Tooltip("Right hand IK settings")]
        public HandSetting rightHand;
    }
    [Tooltip("Hand IK settings")]
    public HandIkSettings handIkSettings;

    [Tooltip("Refer to the documentation. WARNING: These values may cause severe problems if not setup correctly!")]
    public bool applyForearmCorrection = true;
    [Tooltip("Refer to the documentation. WARNING: These values may cause severe problems if not setup correctly!")]
    public Vector3 forearmCorrectionBoneAxis = new Vector3(90, 0, 0);

    public void SaveChanges()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
