using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTransformTargetProvider : MonoBehaviour, IOrbitalTargetProvider
{
    [Tooltip("The Orbital Target the character will aim at.")]
    public OrbitalTarget orbitalTarget;

    //Returns the calculated position when queried
    public Vector3 GetTargetLocation(out bool failed)
    {
        if (orbitalTarget == null)
        {
            failed = false;
            return new Vector3(0,0,-10000);
        }
        failed = false;
        return orbitalTarget.transform.position + orbitalTarget.transform.TransformDirection(orbitalTarget.relativeOffset) + orbitalTarget.globalOffset;
    }
}
