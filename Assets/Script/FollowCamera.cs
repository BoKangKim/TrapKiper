using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Follow CameraArm")]
    [SerializeField] private Transform cameraArmTr = null;
    [SerializeField] private Transform playerTr    = null;

    public float mouseX = 0;
    private float mouseY = 0;

    private void Update()
    {
        GetAxisValue();

        CameraMove();
    }

    private void GetAxisValue()
    {
        //Mouse Axis
        mouseX += Input.GetAxis("Mouse X") * 2;
        mouseY += Input.GetAxis("Mouse Y") * 2;
    }
    void CameraMove()
    {
        //FollowCam
        cameraArmTr.position = playerTr.transform.position;

        if (-mouseY <= -40f)
        {
            mouseY = 39.5f;
            return;
        }
        else if (-mouseY >= 30f)
        {
            mouseY = -29.5f;
        }
        //Camera Rotation
        cameraArmTr.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

}
