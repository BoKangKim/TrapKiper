using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownCircle : MonoBehaviour
{
    private bool downCheck = false;

    private void OnEnable()
    {
        downCheck = false;
    }


    private void FixedUpdate()
    {
        if(!downCheck)
        transform.Translate(0,-1*Time.fixedDeltaTime,0,Space.World);
    }


    private void OnTriggerEnter(Collider other)
    {
        downCheck = true;
    }
}
