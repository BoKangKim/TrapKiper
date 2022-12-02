using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalTarget : MonoBehaviour
{
    [Tooltip("A global offset for the aim target position")]
    public Vector3 globalOffset;
    [Tooltip("A local offset for the aim target position")]
    public Vector3 relativeOffset;
}
