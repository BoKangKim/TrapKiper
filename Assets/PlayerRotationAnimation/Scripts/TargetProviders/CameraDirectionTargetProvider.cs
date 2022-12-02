using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDirectionTargetProvider : MonoBehaviour, IOrbitalTargetProvider
{
    [Serializable]
    public class RaycastSettings
    {
        [Serializable]
        public enum UpdateCycle
        {
            fixedUpdate,
            update,
            lateUpdate
        }

        [Tooltip("Transform of the gameobject that will raycast.")]
        public Transform cameraTransform;
        [Tooltip("Relative offset from the camera for the raycast origin")]
        public Vector3 relativeOffset;
        [Tooltip("What relative direction from the camera the raycast is tested.")]
        public Vector3 direction = Vector3.forward;
        [Tooltip("What distance from the camera that the raycast will test.")]
        public float raycastDistance = 250;
        [Tooltip("Mask for the raycast targeting. Be aware that the player can target itself without specifying it's layer here.")]
        public LayerMask layerMask;
        [Tooltip("The point at which the raycast happens:")]
        public UpdateCycle updateCycle;
    }

    [Header("Raycast Settings (Required!)"), Tooltip("Settings for the camera raycast:")]
    public RaycastSettings raycastSettings;

    [Header("Misc Settings:"), Tooltip("The character will aim towards a location far in front of the camera if the raycast does not hit anything.")]
    public bool aimAtNothing = false;

    [Serializable]
    public class TargetAssistSettings
    {
        [HideInInspector]
        public OrbitalTarget orbitalTarget;
        [Range(0, 100), Tooltip("If this value is set to its max, the character will aim at the value specified by the Orbital Target Script instead of a 0 value, in which the character will aim at the raycast hit point. A value in between 0 and the max is blend of these extremes.")]
        public float assistancePercentage = 0;
        [Tooltip("The player will only aim in a direction if an orbital target was discovered there.")]
        public bool requireTargetToAim = false;
        [Tooltip("Optional: setting this value will prevent the character from aiming at itself. NOTE: if the character has more than one orbital target script attached setup the raycast layer mask instead of using this value.")]
        public OrbitalTarget self;
    }

    [Header("Target Assist Settings"), Tooltip("How the presence of orbital targets affects aiming.")]
    public TargetAssistSettings targetAssistSettings;

    [Serializable]
    public class LocalValues
    {
        public Vector3 raycastReturn;
    }

    [Header("Debug: (Read-Only)")]
    public LocalValues localValues;

    //Returns the calculated position when queried
    public Vector3 GetTargetLocation(out bool failed)
    {
         if (targetAssistSettings.requireTargetToAim && targetAssistSettings.orbitalTarget == null)
        {
            failed = false;
            return new Vector3(0, 0, -10000);
        }
        else if (targetAssistSettings.assistancePercentage == 0 || targetAssistSettings.orbitalTarget == null)
        {
            failed = false;
            return localValues.raycastReturn;
        }
        failed = false;
        Vector3 targetPosition = targetAssistSettings.orbitalTarget.transform.position + targetAssistSettings.orbitalTarget.transform.TransformDirection(targetAssistSettings.orbitalTarget.relativeOffset) + targetAssistSettings.orbitalTarget.globalOffset;
        return Vector3.Lerp(localValues.raycastReturn, targetPosition, targetAssistSettings.assistancePercentage/100);
    }

    //Sets local values
    private void Start()
    {
        if (targetAssistSettings.self == null)
        {
            targetAssistSettings.self = GetComponentInParent<OrbitalTarget>();
        }
    }

    //Preforms the target check raycast based on the update cycle setting
    void FixedUpdate()
    {
        if (raycastSettings.updateCycle != RaycastSettings.UpdateCycle.fixedUpdate) return;
        RaycastCamera();
    }
    //Preforms the target check raycast based on the update cycle setting
    void LateUpdate()
    {
        if (raycastSettings.updateCycle != RaycastSettings.UpdateCycle.lateUpdate) return;
        RaycastCamera();
    }
    //Preforms the target check raycast based on the update cycle setting
    void Update()
    {
        if (raycastSettings.updateCycle != RaycastSettings.UpdateCycle.update) return;
        RaycastCamera();
    }

    //Preforms the target check raycast
    void RaycastCamera()
    {
        if (raycastSettings.cameraTransform == null)
        {
            Debug.LogError("You must set the Camera's transform under the raycast settings!");
            return;
        }

        targetAssistSettings.orbitalTarget = null;

        RaycastHit raycastHit;
        Vector3 origin = raycastSettings.cameraTransform.position + raycastSettings.cameraTransform.TransformDirection(raycastSettings.relativeOffset);
        Vector3 direction = raycastSettings.cameraTransform.TransformDirection(raycastSettings.direction);
        if (Physics.Raycast(origin, direction, out raycastHit, raycastSettings.raycastDistance, raycastSettings.layerMask))
        {
            localValues.raycastReturn = raycastHit.point;
            targetAssistSettings.orbitalTarget = raycastHit.transform.GetComponent<OrbitalTarget>();
            if (targetAssistSettings.self != null && targetAssistSettings.orbitalTarget == targetAssistSettings.self)
            {
                targetAssistSettings.orbitalTarget = null;
            }
            return;
        }
        else if (aimAtNothing)
        {
            localValues.raycastReturn = raycastSettings.cameraTransform.position + raycastSettings.cameraTransform.TransformDirection(raycastSettings.direction * raycastSettings.raycastDistance);
            return;
        }
        //This value will temporally deactivate the aiming system
        localValues.raycastReturn = new Vector3(0,0,-10000);
    }
}
