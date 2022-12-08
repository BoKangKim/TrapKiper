using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionTargetProvider : MonoBehaviour, IOrbitalTargetProvider
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

        [Tooltip("The camera that the raycast will be preformed by.")]
        public Camera camera;

        [Serializable]
        public class ScreenCrop
        {
            [Tooltip("Prevents aiming when the mouse moves 'offscreen'")]
            public bool enabled = false;
            [Tooltip("Defines the lower border of the screen")]
            public float lowerVerticalCrop = 0;
            [Tooltip("Defines the upper border of the screen")]
            public float upperVerticalCrop = 0;
            [Tooltip("Defines the left border of the screen")]
            public float lowerHorizontalCrop = 0;
            [Tooltip("Defines the right border of the screen")]
            public float upperHorizontalCrop = 0;
        }
        [Tooltip("Prevents aiming when the mouse moves 'offscreen'")]
        public ScreenCrop screenCrop;
        [Tooltip("What distance from the camera that the raycast will test.")]
        public float raycastDistance = 250;
        [Tooltip("Mask for the raycast targeting. Be aware that the player can target itself without specifying it's layer here.")]
        public LayerMask layerMask;
        [Tooltip("The point at which the raycast happens:")]
        public UpdateCycle updateCycle;
    }

    [Header("Raycast Settings (Required!)")]
    public RaycastSettings raycastSettings;

    [Serializable]
    public class TargetAssistSettings
    {
        [HideInInspector]
        public OrbitalTarget orbitalTarget;
        [Range(0, 100), Tooltip("If this value is set to its max, the character will aim at the value specified by the Orbital Target Script instead of a 0 value, in which the character will aim at the raycast hit point. A value in between 0 and the max is blend of these extremes.")]
        public float assistancePercentage = 0;
        [Tooltip("The player will only aim in a direction if an orbital target was discovered there.")]
        public bool requireTargetToAim = false;
        [Tooltip("Optional: setting this value will prevent the character from aiming at itself. NOTE: if the character has more than one orbital target script attatched setup the raycast layer mask instead of using this value.")]
        public OrbitalTarget self;
    }

    [Header("Target Assist Settings"), Tooltip("How the presence of oribtal targets affects aiming.")]
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
        return Vector3.Lerp(localValues.raycastReturn, targetPosition, targetAssistSettings.assistancePercentage / 100);
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
        if (raycastSettings.camera == null)
        {
            Debug.LogError("You must set the Camera under the raycast settings!");
            return;
        }

        targetAssistSettings.orbitalTarget = null;

        if (CheckScreenLimits(Input.mousePosition))
        {

            RaycastHit raycastHit;
            Ray ray = raycastSettings.camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, raycastSettings.raycastDistance, raycastSettings.layerMask))
            {
                localValues.raycastReturn = raycastHit.point;
                targetAssistSettings.orbitalTarget = raycastHit.transform.GetComponent<OrbitalTarget>();
                if (targetAssistSettings.self != null && targetAssistSettings.orbitalTarget == targetAssistSettings.self)
                {
                    targetAssistSettings.orbitalTarget = null;
                }
                return;
            }
        }
        //This value will temporally deactivate the aiming system
        localValues.raycastReturn = new Vector3(0, 0, -10000);
    }

    //Checks if the mouse is 'offscreen'
    bool CheckScreenLimits(Vector3 position)
    {
        if (!raycastSettings.screenCrop.enabled) return true;

        if (position.y <= raycastSettings.screenCrop.lowerVerticalCrop)
        {
            return false;
        }
        if (position.y >= Screen.height - raycastSettings.screenCrop.upperVerticalCrop)
        {
            return false;
        }
        if (position.x <= raycastSettings.screenCrop.lowerHorizontalCrop)
        {
            return false;
        }
        if (position.x >= Screen.width - raycastSettings.screenCrop.upperHorizontalCrop)
        {
            return false;
        }

        return true;
    }
}
