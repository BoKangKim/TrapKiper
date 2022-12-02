using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformTargetProvider : MonoBehaviour, IOrbitalTargetProvider
{
    [Tooltip("The transform target the character will aim at.")]
    public Transform target;
    [Tooltip("A global offset for the aim target position")]
    public Vector3 globalOffset;
    [Tooltip("A local offset for the aim target position")]
    public Vector3 relativeOffset;

    //Returns the calculated position when queried
    public Vector3 GetTargetLocation(out bool failed)
    {
        if (target == null)
        {
            failed = false;
            return new Vector3(0, 0, -10000);
        }
        failed = false;
        return target.position + target.TransformDirection(relativeOffset) + globalOffset;
    }
}
