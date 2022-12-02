using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTargetRelay : MonoBehaviour, IOrbitalTargetProvider
{
    [Serializable]
    public enum UsedProvider
    {
        cameraDirectionTargetProvider,
        mousePositionTargetProvider,
        orbitalTransformTargetProvider,
        transformTargetProvider
    }
    [Tooltip("The target provider type that is currently being used. This can be changed at runtime by using the ChangeActiveTargetProvider function!")]
    public UsedProvider activeTargetProvider;

    [Header("Target Provider Options:"),Tooltip("Optional: the camera direction target provider. Only one target provider should be on the exoskeleton! (This includes the relay!). Other target providers should be on a different gameobject!")]
    public CameraDirectionTargetProvider cameraDirectionTargetProvider;
    [Tooltip("Optional: the mouse position target provider. Only one target provider should be on the exoskeleton! (This includes the relay!). Other target providers should be on a different gameobject!")]
    public MousePositionTargetProvider mousePositionTargetProvider;
    [Tooltip("Optional: the orbital transform target provider. Only one target provider should be on the exoskeleton! (This includes the relay!). Other target providers should be on a different gameobject!")]
    public OrbitalTransformTargetProvider orbitalTransformTargetProvider;
    [Tooltip("Optional: the transform target provider. Only one target provider should be on the exoskeleton! (This includes the relay!). Other target providers should be on a different gameobject!")]
    public TransformTargetProvider transformTargetProvider;

    //Sends the query through to the active target provider
    public Vector3 GetTargetLocation(out bool failed)
    {
        if (activeTargetProvider == UsedProvider.cameraDirectionTargetProvider && cameraDirectionTargetProvider != null)
        {
            return cameraDirectionTargetProvider.GetTargetLocation(out failed);
        }
        if (activeTargetProvider == UsedProvider.mousePositionTargetProvider && mousePositionTargetProvider != null)
        {
            return mousePositionTargetProvider.GetTargetLocation(out failed);
        }
        if (activeTargetProvider == UsedProvider.orbitalTransformTargetProvider && orbitalTransformTargetProvider != null)
        {
            return orbitalTransformTargetProvider.GetTargetLocation(out failed);
        }
        if (activeTargetProvider == UsedProvider.transformTargetProvider && transformTargetProvider != null)
        {
            return transformTargetProvider.GetTargetLocation(out failed);
        }
        failed = true;
        return new Vector3(0, 0, -10000);
    }
    //Changes the active target provider
    public void ChangeActiveTargetProvider(UsedProvider newProvider)
    {
        activeTargetProvider = newProvider;
    }

}
