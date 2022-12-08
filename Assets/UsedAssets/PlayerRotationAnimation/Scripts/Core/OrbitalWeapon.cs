using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalWeapon : MonoBehaviour
{
    [Tooltip("The bind point for the left hand's IK target relative to the weapon")]
    public Transform bindPoint;
    [Tooltip("The weapon profile. Refer to the documentation for more information.")]
    public OrbitalWeaponProfile weaponProfile;

    [Serializable]
    public enum WeaponState
    {
        Locked,
        Unlocked,
        Undefined
    }

    //The current state of the weapon
    [HideInInspector]
    public WeaponState weaponState = WeaponState.Unlocked;


    //Updates the weapons transform based on its state
    private void Update()
    {
        if (weaponState == WeaponState.Locked)
        {
            UpdateLockedTransform();
        }
        if (weaponState == WeaponState.Unlocked)
        {
            if (weaponProfile.useUnlockedSettings)
            {
                UpdateUnlockedTransform();
            }
            else
            {
                SetUndefinedGunTransform();
            }
        }
    }

    //Changes the weapon's parent during the latch event
    public void SetLockedGunTransform(Transform parent)
    {
        if (weaponProfile == null)
        {
            Debug.LogError("Weapon is missing Orbital Weapon Transform Settings!");
            return;
        }
        transform.parent = parent;
        weaponState = WeaponState.Locked;
    }
    //Updates the transform of the locked weapon
    public void UpdateLockedTransform()
    {
        if (weaponProfile == null)
        {
            Debug.LogError("Weapon is missing Orbital Weapon Transform Settings!");
            return;
        }
        SetRelativePosition(weaponProfile.lockedGunSettings.positionOffset);
        SetRelativeRotation(weaponProfile.lockedGunSettings.rotationOffset);
    }
    //Changes the weapon's parent during the unlatch event
    public void SetUnlockedGunTransform(Transform parent)
    {
        if (weaponProfile == null)
        {
            Debug.LogError("Weapon is missing Orbital Weapon Transform Settings!");
            return;
        }
        transform.parent = parent;
        if (weaponProfile.useUnlockedSettings)
        {
            weaponState = WeaponState.Unlocked;
        }
        else
        {
            SetUndefinedGunTransform();
        }
    }
    //Updates the transform of the unlocked weapon
    public void UpdateUnlockedTransform()
    {
        if (weaponProfile == null)
        {
            Debug.LogError("Weapon is missing Orbital Weapon Transform Settings!");
            return;
        }
        SetRelativePosition(weaponProfile.unlockedGunSettings.positionOffset);
        SetRelativeRotation(weaponProfile.unlockedGunSettings.rotationOffset);
    }
    //Stops the weapon from updating its transform
    public void SetUndefinedGunTransform()
    {
        weaponState = WeaponState.Undefined;
    }
    //Sets a transforms local position
    void SetRelativePosition(Vector3 offset)
    {
        gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, offset /100, weaponProfile.adjustmentSpeed*Time.deltaTime);
    }
    //Sets a transforms local rotation
    void SetRelativeRotation(Vector3 offset)
    {
        Quaternion newRot = Quaternion.identity;
        newRot.eulerAngles = offset;
        gameObject.transform.localRotation = Quaternion.Lerp(gameObject.transform.localRotation, newRot, weaponProfile.adjustmentSpeed * Time.deltaTime);
    }
    //Returns the bind point's position
    public Vector3 GetBindPointPosition()
    {
        return bindPoint.position;
    }
}
