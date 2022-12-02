using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Provides the target position when queryed by the orbital targeting script
public interface IOrbitalTargetProvider
{
    Vector3 GetTargetLocation(out bool failed);
}
