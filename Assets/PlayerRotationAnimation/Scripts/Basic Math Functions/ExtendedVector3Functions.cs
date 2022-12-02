using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtendedVector3Functions
{
    //Sets the relative Y value of a world space vector to 0
    public static Vector3 RemoveRelativeVector3Y(Transform transform, Vector3 vector3)
    {
        Vector3 returnVector = transform.InverseTransformPoint(vector3);
        returnVector.y = 0;
        returnVector = transform.TransformPoint(returnVector);
        return returnVector;
    }

    //Sets the relative X value of a world space vector to 0
    public static Vector3 RemoveRelativeVector3X(Transform transform, Vector3 vector3)
    {
        Vector3 returnVector = transform.InverseTransformPoint(vector3);
        returnVector.x = 0;
        returnVector = transform.TransformPoint(returnVector);
        return returnVector;
    }

    //Sets the relative Z value of a world space vector to 0
    public static Vector3 RemoveRelativeVector3Z(Transform transform, Vector3 vector3)
    {
        Vector3 returnVector = transform.InverseTransformPoint(vector3);
        returnVector.z = 0;
        returnVector = transform.TransformPoint(returnVector);
        return returnVector;
    }

    //Find the average vector 3 value of a vector 3 list
    public static Vector3 AverageVector3List(List<Vector3> vectorList)
    {
        Vector3 returnVector = Vector3.zero;
        for (int i = 0; i < vectorList.Count; i++)
        {
            returnVector += vectorList[i];
        }
        returnVector = returnVector / vectorList.Count;
        return returnVector;
    }
}
