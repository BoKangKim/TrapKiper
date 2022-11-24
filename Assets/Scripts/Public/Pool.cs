using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pool
{
    /// <summary>
    /// (KEY)<string> -> Object's Name , (VALUE)<Queue> -> Containing Pooling objects
    /// </summary>
    private static Dictionary<string, Queue<GameObject>> listCache = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// Standard Object Pooling
    /// </summary>
    /// <param name="instObj"> Object to be created </param>
    /// <param name="position"> Position to be created </param>
    /// <param name="rotation"> Rotation to be created </param>
    /// <returns> Pooled Object or Created Object </returns>
    public static GameObject ObjectInstantiate(GameObject instObj, Vector3 position, Quaternion rotation)
    {
        string objectID = instObj.name;

        Queue<GameObject> que = null;

        ListCaching(instObj);

        bool listCached = listCache.TryGetValue(objectID, out que);

        if (listCached == false)
        {
            Debug.LogError("Can not Add " + objectID + " Please Request BK");
            return null;
        }

        GameObject inst = null;

        if (que.Count == 0)
        {
            inst = MonoBehaviour.Instantiate(instObj, position, rotation);
        }
        else
        {
            inst = que.Dequeue();
            inst.transform.position = position;
            inst.transform.rotation = rotation;
        }

        inst.SetActive(true);
        return inst;
    }

    /// <summary>
    /// Overloading Standard Pooling for Set Object's Parent 
    /// </summary>
    /// <param name="instObj"> Object to be created </param>
    /// <param name="parent"> instObj's parent </param>
    /// <param name="worldPositionStays"> If true, the parent-relative position, scale and rotation are modified such that the object keeps the same world space position, rotation and scale as before. </param>
    /// <returns> Pooled Object or Created Object </returns>
    public static GameObject ObjectInstantiate(GameObject instObj, Transform parent, bool worldPositionStays)
    {
        instObj = ObjectInstantiate(instObj, instObj.transform.position, instObj.transform.rotation);

        instObj.transform.SetParent(parent, worldPositionStays);

        return instObj;
    }

    /// <summary>
    /// SetActived false Obj and Enqueue for instObj Queue 
    /// </summary>
    /// <param name="instObj"> Request Destroy Obj </param>
    public static void ObjectDestroy(GameObject instObj)
    {
        string objectID = instObj.name.Replace("(Clone)", "");

        Queue<GameObject> que = null;
        bool listCached = listCache.TryGetValue(objectID, out que);
        if (listCached == false)
        {
            Debug.LogError(objectID + " is not using pool Object");
            return;
        }

        instObj.SetActive(false);
        que.Enqueue(instObj);
    }

    /// <summary>
    /// Cashing Before Using Instantiate
    /// </summary>
    /// <param name="instObj"> cashing obj </param>
    public static void ListCaching(GameObject instObj)
    {
        string objectID = instObj.name;

        bool listCached = listCache.ContainsKey(objectID);
        if (listCached == false)
        {
            listCache.Add(objectID, new Queue<GameObject>());
        }
        else
            return;
    }

    /// <summary>
    /// Overloading single caching for array caching
    /// </summary>
    /// <param name="instObj"> Caching obj array </param>
    public static void ListCaching(GameObject[] instObj)
    {
        string[] objectID = new string[instObj.Length];

        for (int i = 0; i < objectID.Length; i++)
        {
            objectID[i] = instObj[i].name;

            bool listCached = listCache.ContainsKey(objectID[i]);

            if (listCached == false)
            {
                listCache.Add(objectID[i], new Queue<GameObject>());
            }
            else
                continue;
        }
    }

}
