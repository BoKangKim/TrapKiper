using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Default Weapon Profile", menuName = "Orbital Aiming System/Weapon Profile", order = 1)]
public class OrbitalWeaponProfile : ScriptableObject
{
    [Header("Animation Settings:"), Range(0, 0.99f), Tooltip("This value defines the hard latching point of the weapon as it transitions from the unlatched weapon parent transform to the latched weapon parent transform, based on IK weight.")]
    public float weaponGate = 0.2f;
    [Tooltip("The movement speed of the weapon as it moves from the latched and unlatched positions and rotations.")]
    public float adjustmentSpeed = 10f;

    [Serializable]
    public class LockedGunSettings
    {
        public Vector3 positionOffset = new Vector3(3.428252f, -0.447849f, -1.083687f);
        public Vector3 rotationOffset = new Vector3(-8.12028f, 84.01889f, 92.61794f);
    }
    [Serializable]
    public class UnlockedGunSettings
    {
        public Vector3 positionOffset = new Vector3(0.02010167f, 0.06315529f, 0.02205014f);
        public Vector3 rotationOffset = new Vector3(-85.85003f, 106.8633f, 80.4358f);
    }

    [Header("Transform Settings:"), Tooltip("The relative position and rotation of the weapon when in the latched ready state.")]
    public LockedGunSettings lockedGunSettings;
    [Tooltip("Should the position and rotation of the gun be set when not in the latched position? Set this value to false if you wish to control the weapon's transform yourself when it's not in the ready position.")]
    public bool useUnlockedSettings = true;
    [Tooltip("The relative position and rotation of the weapon when in the unlatched state.")]
    public UnlockedGunSettings unlockedGunSettings;

    [Serializable]
    public class HandIkSettings
    {
        [Serializable]
        public class LeftHandSetting
        {
            public Vector3 positionOffset = new Vector3(-1.088403f, -3.662221f, 0.2980752f);
            public Vector3 rotationOffset = new Vector3(9.198089f, -36.79242f, 87.38197f);
        }
        [Serializable]
        public class RightHandSetting
        {
            public Vector3 positionOffset = new Vector3(0.1100564f, 1.422958f, -1.933959f);
            public Vector3 rotationOffset = new Vector3(16.86326f, -95.04722f, 165.5661f);
        }
        [Tooltip("The position and rotation of the left hand relative to the gun.")]
        public LeftHandSetting leftHand;
        [Tooltip("The position and rotation of the right hand relative to the gun.")]
        public RightHandSetting rightHand;
    }
    [Header("Hand IK Settings:"), Tooltip("The position and rotation of the hands relative to the gun.")]
    public HandIkSettings handIkSettings;

    [Serializable]
    public class FingerPositionSettings
    {
        [Serializable]
        public class LeftHand
        {
            [Range(0, 2), Tooltip("Controls the left hand’s (not trigger hand) finger orientation.")]
            public float HandPositionBlend = 1;
            [Range(0, 0.99f), Tooltip("Controls the openness of the grasp of the position 2 hand orientation but will also have some minor effects in the blend between the position 1 and 2 hand orientations. Remember, the hand can be rotated entirely through the IK settings.")]
            public float HandOpenness = 0;
        }
        [Serializable]
        public class RightHand
        {
            [Range(0, 1), Tooltip("Controls the right hand’s (trigger hand) finger orientation.")]
            public float HandPositionBlend = 0;
        }
        [Tooltip("Controls the left hand’s (not trigger hand) finger orientation.")]
        public LeftHand leftHand;
        [Tooltip("Controls the right hand’s (trigger hand) finger orientation.")]
        public RightHand rightHand;
    }
    [Header("Finger Position Settings:"), Tooltip("Controls the hands' finger orientations.")]
    public FingerPositionSettings fingerPositionSettings;

    [Serializable]
    public class Recoil
    {
        [Range(0, 360), Tooltip("The minimum intensity of a single shot. Refer to the documentation for more information.")]
        public float minimumShotMagnitude = 40;
        [Range(0, 360), Tooltip("The maximum intensity of a single shot. Refer to the documentation for more information.")]
        public float maximumShotMagnitude = 50;
        [Tooltip("How quickly the character reacts to a higher set recoil value. Refer to the documentation for more information.")]
        public float entrySpeed = 60f;
        [Tooltip("The speed at which recoil deviation is corrected. Refer to the documentation for more information.")]
        public float recoverySpeed = 80;
        [Tooltip("The speed of the animation that visualizes backward force.")]
        public float animationSpeed = 1.35f;
        [Serializable]
        public class RecoilWeight
        {
            [Serializable]
            public class Weight
            {
                [Tooltip("Is this direction used?")]
                public bool enabled = true;
                [Range(0, 180), Tooltip("The lower limit of the random calculation for this direction.")]
                public float lowerLimit;
                [Range(0, 180), Tooltip("The upper limit of the random calculation for this direction.")]
                public float upperLimit;
            }
            [Tooltip("The controls for the random intensity of left directional recoil.")]
            public Weight left;
            [Tooltip("The controls for the random intensity of right directional recoil.")]
            public Weight right;
            [Tooltip("The controls for the random intensity of upwards directional recoil.")]
            public Weight up;
            [Tooltip("The controls for the random intensity of downwards directional recoil.")]
            public Weight down;
            [Range(0,1), Tooltip("The intensity of the backwards movement animation. Refer to the documentation for more information.")]
            public float kick = 1;
        }

        [Tooltip("These values control directional recoil movement and animation intensity. Refer to the documentation for more information.")]
        public RecoilWeight recoilWeight;

        [Serializable]
        public class HardCap
        {
            [Tooltip("Is the hard cap enabled? Refer to the documentation for more information.")]
            public bool enabled = true;
            [Tooltip("The limit of the hard cap. Refer to the documentation for more information.")]
            public float value = 50;
        }

        [Serializable]
        public class SoftCap
        {
            [Tooltip("Is the soft cap enabled? Refer to the documentation for more information.")]
            public bool enabled = true;
            [Tooltip("The limit of the soft cap. Refer to the documentation for more information.")]
            public float value = 40;
            [Tooltip("The softness of the soft cap. Recoil values above the limit will be multiplied by this value. Refer to the documentation for more information.")]
            public float softness = 0.8f;
        }
        [Tooltip("Soft limits for the amount of additive recoil deviation allowed across multiple shots, depending on recovery speed. Refer to the documentation for more information.")]
        public SoftCap persistentSoftCap;
        [Tooltip("Hard limits for the amount of additive recoil deviation allowed across multiple shots, depending on recovery speed. Refer to the documentation for more information.")]
        public HardCap persistentHardCap;
    }
    [Header("Recoil Settings:"), Tooltip("Controls for animated recoil. Refer to the documentation for more information.")]
    public Recoil recoil;

    [Header("Projectile Drop Correction Settings:"), Tooltip("The amount the character will aim further upward to correct for projectile drop over a distance.")]
    public float projectileDropCorrection;
    [Range(0,89f), Tooltip("The upward angle limit for projectile drop correction with the intent that room for recoil is available before hitting the gyro limit. If the x axis gyro's angle limit is smaller than this value, the gyro limit will be used instead.")]
    public float correctionMaximumAngleLimit;

    public void SaveChanges()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}
