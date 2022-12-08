using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OrbitalRemote : MonoBehaviour
{
    [Header("Character Settings: (Required!)"), Tooltip("Is the character's hand IK enabled?")]
    public bool iKEnabled = true;
    [Tooltip("The active character profile used.")]
    public OrbitalCharacterProfile characterSettings;

    [Serializable]
    public class RequiredTransforms
    {
        [Serializable]
        public class LeftSide
        {
            [Tooltip("The character's left hand.")]
            public Transform hand;
            [Tooltip("The character's left hand's middle finger's knuckle.")]
            public Transform middleFingerBase;
        }
        [Serializable]
        public class RightSide
        {
            [Tooltip("The character's right hand.")]
            public Transform hand;
            [Tooltip("The character's right hand's middle finger's first joint after the knuckle.")]
            public Transform middleFingerFirstJoint;
        }
        [Tooltip("The character's left side required transforms.")]
        public LeftSide leftSide;
        [Tooltip("The character's right side required transforms.")]
        public RightSide rightSide;
        [Tooltip("The transform that the weapon will be a child of when the character is not in the aiming state. This is usually the player's right hand. Refer to the documentation for more information.")]
        public Transform unlatchedWeaponParent;
    }
    [Header("Required Settings: "), Tooltip("Specific Armature Transforms that need to be assigned.")]
    public RequiredTransforms requiredTransforms;


    [Serializable]
    public class RecommendedSettings
    {
        [Tooltip("The oribtal targeting script located on the exoskeleton. Refer to the documentation for more information.")]
        public OrbitalTargeting orbitalTargeting;
        [Tooltip("The character's animator.")]
        public Animator animator;
    }

    [Header("Recommended Settings: "), Tooltip("In most situations, it is not required to assign these values, but it is recommended.")]
    public RecommendedSettings recommendedSettings;

    [Serializable]
    public class WeaponBasedEvents
    {  
        [Tooltip("This event is called when the weapon latches during the animation transition into the aiming state.")]
        public UnityEvent OnWeaponLatch;
        [Tooltip("This event is called when the weapon unlatches during the animation transition out of the aiming state.")]
        public UnityEvent OnWeaponUnlatch;
    }

    [Header("Events: "), Tooltip("These events are called in the animation transitions in and out of the aiming state.")]
    public WeaponBasedEvents weaponBasedEvents;

    [Serializable]
    public class LocalValues
    {
        public float currentIkWeight = 1;
        public float targetIkWeight = 1;
        public float iKAdjustmentSpeed = 1;
        public OrbitalWeapon currentWeapon;
        public OrbitalRemoteSignal signal;
        public Vector3 handScale;
    }

    [Header("Debug: (Read-Only)")]
    public LocalValues localValues;

    enum ChangeType
    {
        Positive,
        Negative,
        Equal
    }

    // Start is called before the first frame update
    void Start()
    {
        //Finds the animator if needed
        CheckAnimator();
        //Finds the orbital targeting script if needed
        CheckOrbitalTargeting();
        //Get's the starting information from the orbital targeting script
        GenerateSignal();
        //Set's the hand scale for forearm twist correction purposes
        localValues.handScale = requiredTransforms.leftSide.hand.localScale;
    }
    //Finds the animator if needed
    void CheckAnimator()
    {
        if (recommendedSettings.animator == null)
        {
            recommendedSettings.animator = GetComponent<Animator>();
        }
    }
    //Finds the orbital targeting script if needed
    void CheckOrbitalTargeting()
    {
        if (recommendedSettings.orbitalTargeting == null)
        {
            recommendedSettings.orbitalTargeting = GetComponentInChildren<OrbitalTargeting>();
        }
    }
    //Get's the starting information from the orbital targeting script
    void GenerateSignal()
    {
        if (recommendedSettings.orbitalTargeting == null) return;
        localValues.signal = new OrbitalRemoteSignal();
        recommendedSettings.orbitalTargeting.GetSystemInformation(ref localValues.signal, true);
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the orbital targeting signal
        UpdateSignal();
        //Updates the character's animator
        UpdateAnimator();
        //Manages the weapon's states
        CheckWeaponGate();
    }
    //Updates the orbital targeting signal
    void UpdateSignal()
    {
        if (recommendedSettings.orbitalTargeting == null) return;
        recommendedSettings.orbitalTargeting.GetSystemInformation(ref localValues.signal);
    }
    //Updates the character's animator
    void UpdateAnimator()
    {
        recommendedSettings.animator.SetFloat("OrbitalYGyroRotation", localValues.signal.yGyroRotation);
        recommendedSettings.animator.SetFloat("OrbitalXGyroRotation", localValues.signal.xGyroRotation);

        if (localValues.currentWeapon == null) return;
        recommendedSettings.animator.SetFloat("OrbitalLeftHand", localValues.currentWeapon.weaponProfile.fingerPositionSettings.leftHand.HandPositionBlend);
        recommendedSettings.animator.SetFloat("OrbitalRightHand", localValues.currentWeapon.weaponProfile.fingerPositionSettings.rightHand.HandPositionBlend);
        recommendedSettings.animator.SetFloat("OrbitalLeftHandOpenness", localValues.currentWeapon.weaponProfile.fingerPositionSettings.leftHand.HandOpenness);
    }
    //Checks if some of the required transforms have been set.
    bool CheckTransforms()
    {
        if (requiredTransforms.leftSide.hand == null)
        {
            Debug.LogError("Required Transform is missing!");
            return false;
        }
        if (requiredTransforms.rightSide.hand == null)
        {
            Debug.LogError("Required Transform is missing!");
            return false;
        }
        if (requiredTransforms.leftSide.middleFingerBase == null)
        {
            Debug.LogError("Required Transform is missing!");
            return false;
        }
        if (requiredTransforms.rightSide.middleFingerFirstJoint == null)
        {
            Debug.LogError("Required Transform is missing!");
            return false;
        }
        return true;
    }
    //Manages the weapon's states based on the desired IK weight and direction of IK weight change
    void CheckWeaponGate()
    {
        float iKWeightChange = Mathf.Clamp01(Mathf.Lerp(localValues.currentIkWeight, localValues.targetIkWeight, localValues.iKAdjustmentSpeed * Time.deltaTime));


        ChangeType changeType = ChangeType.Equal;
        if (iKWeightChange > localValues.currentIkWeight)
        {
            changeType = ChangeType.Positive;
        }
        if (iKWeightChange < localValues.currentIkWeight)
        {
            changeType = ChangeType.Negative;
        }

        localValues.currentIkWeight = iKWeightChange;
        if (changeType == ChangeType.Positive && localValues.currentWeapon != null && !recommendedSettings.orbitalTargeting.CheckForWeapon() && localValues.currentIkWeight > localValues.currentWeapon.weaponProfile.weaponGate)
        {
            //Latches the gun and calls the latch event
            recommendedSettings.orbitalTargeting.SetGun(localValues.currentWeapon);
            weaponBasedEvents.OnWeaponLatch.Invoke();

        }
        if (changeType == ChangeType.Negative && recommendedSettings.orbitalTargeting.CheckForWeapon())
        {
            //Unlatches the gun and calls the unlatch event
            UnlatchWeapon();
            weaponBasedEvents.OnWeaponUnlatch.Invoke();
        }

        if (changeType == ChangeType.Equal)
        {
            if (localValues.currentIkWeight == 1 && localValues.currentWeapon != null && !recommendedSettings.orbitalTargeting.CheckForWeapon())
            {
                //Latches the gun and calls the latch event
                recommendedSettings.orbitalTargeting.SetGun(localValues.currentWeapon);
                weaponBasedEvents.OnWeaponLatch.Invoke();
            }
            if (localValues.currentIkWeight == 0 && recommendedSettings.orbitalTargeting.CheckForWeapon())
            {
                //Unlatches the gun and calls the unlatch event
                UnlatchWeapon();
                weaponBasedEvents.OnWeaponUnlatch.Invoke();
            }
        }
     }
    //Manages character hand IK
    void OnAnimatorIK(int layerIndex)
    {
        //If IK is not enabled or the some parameters have not been set return
        if ((!iKEnabled) || !CheckTransforms() || localValues.signal.setOrbitalWeapon == null || characterSettings == null)
        {
            SetHandWeight(0);
            return;
        }
        //Sets the hand IK's weight
        SetHandWeight(localValues.currentIkWeight);
        //Set's the left hand IK target
        SetHandIkTransform(true, requiredTransforms.leftSide.hand, requiredTransforms.leftSide.middleFingerBase, localValues.signal.leftHandContrast, localValues.signal.setOrbitalWeapon, localValues.signal.setOrbitalWeapon.bindPoint);
        //Set's the right hand IK target
        SetHandIkTransform(false, requiredTransforms.rightSide.hand, requiredTransforms.rightSide.middleFingerFirstJoint, localValues.signal.rightHandContrast, localValues.signal.setOrbitalWeapon, localValues.signal.setOrbitalWeapon.transform);
    }
    //Sets the hand IK's weight
    void SetHandWeight(float value)
    {
        recommendedSettings.animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, value);
        recommendedSettings.animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, value);

        recommendedSettings.animator.SetIKPositionWeight(AvatarIKGoal.RightHand, value);
        recommendedSettings.animator.SetIKRotationWeight(AvatarIKGoal.RightHand, value);
    }
    //Set's a specified hand IK target
    void SetHandIkTransform(bool isLeft, Transform hand, Transform contrastPoint, float contrastPercent,  OrbitalWeapon orbitalWeapon, Transform bindPoint)
    {
        Vector3 offset = Vector3.Lerp(hand.position, contrastPoint.position, contrastPercent) - hand.position;
        if (isLeft)
        {
            Vector3 calcOffset = orbitalWeapon.transform.TransformDirection(orbitalWeapon.weaponProfile.handIkSettings.leftHand.positionOffset / 100);
            calcOffset += orbitalWeapon.transform.TransformDirection(characterSettings.handIkSettings.leftHand.positionOffset / 100);
            recommendedSettings.animator.SetIKPosition(AvatarIKGoal.LeftHand, bindPoint.position - offset + calcOffset);
        }
        else
        {
            Vector3 calcOffset = orbitalWeapon.transform.TransformDirection(orbitalWeapon.weaponProfile.handIkSettings.rightHand.positionOffset / 100);
            calcOffset += orbitalWeapon.transform.TransformDirection(characterSettings.handIkSettings.rightHand.positionOffset / 100);
            recommendedSettings.animator.SetIKPosition(AvatarIKGoal.RightHand, bindPoint.position - offset + calcOffset);
        }

        Quaternion handRotation = orbitalWeapon.transform.localRotation;

        if (isLeft)
        {
            Vector3 newRotation = handRotation.eulerAngles;
            newRotation = newRotation + orbitalWeapon.weaponProfile.handIkSettings.leftHand.rotationOffset + characterSettings.handIkSettings.leftHand.rotationOffset;
            handRotation.eulerAngles = newRotation;
            handRotation = orbitalWeapon.transform.rotation * handRotation;
            recommendedSettings.animator.SetIKRotation(AvatarIKGoal.LeftHand, handRotation);
        }
        else
        {
            Vector3 newRotation = handRotation.eulerAngles;
            newRotation = newRotation + orbitalWeapon.weaponProfile.handIkSettings.rightHand.rotationOffset + characterSettings.handIkSettings.rightHand.rotationOffset;
            handRotation.eulerAngles = newRotation;
            handRotation = orbitalWeapon.transform.rotation * handRotation;
            recommendedSettings.animator.SetIKRotation(AvatarIKGoal.RightHand, handRotation);
        }
    }
    //Unlatches the weapon
    public void UnlatchWeapon()
    {
        recommendedSettings.orbitalTargeting.UnlatchGun(requiredTransforms.unlatchedWeaponParent);
    }

    private void LateUpdate()
    {
        //Fixes the left forearm rotation
        FixArmRotation();
    }
    //Fixes the left forearm rotation
    void FixArmRotation()
    {
        if (characterSettings != null && !characterSettings.applyForearmCorrection) return;

        Transform forearm = requiredTransforms.leftSide.hand.parent;
        Transform upperArm = forearm.parent;
        requiredTransforms.leftSide.hand.parent = null;
        forearm.parent = null;

        Vector3 upperCalc = recommendedSettings.orbitalTargeting.GetTwistTarget().position - forearm.position;
        Vector3 forwardCalc = requiredTransforms.leftSide.hand.position - forearm.position;
        Quaternion axisConstant = Quaternion.identity;
        axisConstant.eulerAngles = characterSettings.forearmCorrectionBoneAxis;
        Quaternion lookRotation = Quaternion.LookRotation(forwardCalc, upperCalc) * axisConstant;

        forearm.rotation = Quaternion.Lerp(forearm.rotation, lookRotation, localValues.currentIkWeight);

        requiredTransforms.leftSide.hand.parent = forearm;
        forearm.parent = upperArm;
        requiredTransforms.leftSide.hand.localScale = localValues.handScale;
    }
    //Activates aim mode
    public void ActivateAimMode(float speed)
    {
        localValues.targetIkWeight = 1;
        localValues.iKAdjustmentSpeed = speed;
    }
    //Deactivates aim mode
    public void DeactivateAimMode(float speed)
    {
        localValues.targetIkWeight = 0;
        localValues.iKAdjustmentSpeed = speed;
    }
    //Sets a startup delay
    public void SetStartupDelay(float time)
    {
        recommendedSettings.orbitalTargeting.SetStartupDelay(time);
    }
    //Plays the fire animation and applies recoil
    public void PlayFireAnimation(bool applyRecoil)
    {
        recommendedSettings.orbitalTargeting.PlayFireAnimation(recommendedSettings.animator);


        recommendedSettings.orbitalTargeting.PlayFireAnimation();

        if (applyRecoil)
            recommendedSettings.orbitalTargeting.ApplyRecoil();
    }
    //Spawns a weapon from a prefab
    public void SpawnWeaponFromPrefab(GameObject prefab, bool destroyReplacedGun = true)
    {
        if (prefab == null)
        {
            Debug.LogError("Weapon Creation Failed");
        }
        GameObject newWeapon = Instantiate(prefab) as GameObject;
        newWeapon.transform.parent = requiredTransforms.unlatchedWeaponParent;
        OrbitalWeapon ow = newWeapon.GetComponent<OrbitalWeapon>();
        if (ow == null)
        {
            Destroy(newWeapon);
            Debug.LogError("Weapon Creation Failed");
            return;
        }
        if (ow.weaponProfile.useUnlockedSettings)
        {
            newWeapon.transform.localPosition = ow.weaponProfile.unlockedGunSettings.positionOffset/100;
            Quaternion newRotation = Quaternion.identity;
            newRotation.eulerAngles = ow.weaponProfile.unlockedGunSettings.rotationOffset;
            newWeapon.transform.localRotation = newRotation;
        }
        else
        {
            newWeapon.transform.position = requiredTransforms.unlatchedWeaponParent.position;
            newWeapon.transform.rotation = requiredTransforms.unlatchedWeaponParent.rotation;
        }

        GameObject destroyedObject = null;
        if (destroyReplacedGun && localValues.currentWeapon != null)
        {
            destroyedObject = localValues.currentWeapon.gameObject;
        }

        Transform unlatchedGun;
        SetWeapon(ow, out unlatchedGun);
        if (unlatchedGun != null && destroyReplacedGun)
        {
            Destroy(unlatchedGun.gameObject);
        }

        if (destroyedObject != null && destroyReplacedGun)
        {
            Destroy(destroyedObject);
        }
    }
    //Spawns a weapon from a prefab and outputs the new object
    public void SpawnWeaponFromPrefab(GameObject prefab, out GameObject spawnedGameobject, bool destroyReplacedGun = true)
    {
        if (prefab == null)
        {
            Debug.LogError("Weapon Creation Failed");
        }
        GameObject newWeapon = Instantiate(prefab) as GameObject;
        newWeapon.transform.parent = requiredTransforms.unlatchedWeaponParent;
        OrbitalWeapon ow = newWeapon.GetComponent<OrbitalWeapon>();
        if (ow == null)
        {
            Destroy(newWeapon);
            Debug.LogError("Weapon Creation Failed");
            spawnedGameobject = null;
            return;
        }
        if (ow.weaponProfile.useUnlockedSettings)
        {
            newWeapon.transform.localPosition = ow.weaponProfile.unlockedGunSettings.positionOffset / 100;
            Quaternion newRotation = Quaternion.identity;
            newRotation.eulerAngles = ow.weaponProfile.unlockedGunSettings.rotationOffset;
            newWeapon.transform.localRotation = newRotation;
        }
        else
        {
            newWeapon.transform.position = requiredTransforms.unlatchedWeaponParent.position;
            newWeapon.transform.rotation = requiredTransforms.unlatchedWeaponParent.rotation;
        }
        spawnedGameobject = newWeapon;

        GameObject destroyedObject = null;
        if (destroyReplacedGun && localValues.currentWeapon != null)
        {
            destroyedObject = localValues.currentWeapon.gameObject;
        }

        Transform unlatchedGun;
        SetWeapon(ow, out unlatchedGun);

        if (unlatchedGun != null && destroyReplacedGun)
        {
            Destroy(unlatchedGun.gameObject);
        }
        if (destroyedObject != null && destroyReplacedGun)
        {
            Destroy(destroyedObject);
        }
    }
    //Latches the weapon that is currently a child of the unlatched weapon parent transform
    public void SetWeaponInHand()
    {
        OrbitalWeapon orbitalWeapon = requiredTransforms.unlatchedWeaponParent.GetComponentInChildren<OrbitalWeapon>();
        if (orbitalWeapon == null)
        {
            Debug.Log("Weapon not found in hand");
            return;
        }

        Transform unlatchedGun;
        SetWeapon(orbitalWeapon, out unlatchedGun);
    }
    //Sets a weapon and outputs the unlatched weapon's transform
    public void SetWeapon(OrbitalWeapon orbitalWeapon, out Transform unlatchedGun)
    {
        if (recommendedSettings.orbitalTargeting.CheckForWeapon())
        {
            unlatchedGun = recommendedSettings.orbitalTargeting.GetWeaponTransform();
            recommendedSettings.orbitalTargeting.UnlatchGun(requiredTransforms.unlatchedWeaponParent);
        }
        else
        {
            unlatchedGun = null;
        }
        localValues.currentWeapon = orbitalWeapon;
    }
    //Set's a different character profile
    public void SetCharacterSettings(OrbitalCharacterProfile newSettings)
    {
        characterSettings = newSettings;
    }
    //Enables or disables setup mode
    public void SetSetupMode(bool condition)
    {
        recommendedSettings.orbitalTargeting.setupMode = condition;
    }
    //Changes the active target provider if the remote can find a relay on the exoskeleton
    public void AttemptChangeTargetProvider(OrbitalTargetRelay.UsedProvider newProvider)
    {
        OrbitalTargetRelay relay = recommendedSettings.orbitalTargeting.transform.GetComponent<OrbitalTargetRelay>();
        if (relay == null)
        {
            Debug.LogWarning("Relay was not found.");
            return;
        }
        relay.ChangeActiveTargetProvider(newProvider);
    }
    //Return the assigned weapon
    public OrbitalWeapon GetWeapon()
    {
        return localValues.currentWeapon;
    }
}

//Bulk information received from the orbital targeting script
[Serializable]
public class OrbitalRemoteSignal
{
    public float yGyroRotation= 0;
    public float xGyroRotation =0;
    public OrbitalWeapon setOrbitalWeapon;

    public float leftHandContrast =0 ;
    public float rightHandContrast =0;
}
