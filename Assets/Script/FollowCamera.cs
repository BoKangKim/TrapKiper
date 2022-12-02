using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraArmTr = null;
    [SerializeField] private Transform playerTr    = null;

    public float mouseX = 0;
    public float mouseY = 0;

    private float dis   = 0;

    private RaycastHit hit;
    private Vector3 dir;

    private void Update()
    {
        GetAxisValue();

        FollowCam();

        CameraSpring();
    }

    //Mouse Axis
    private void GetAxisValue()
    {
        mouseX += Input.GetAxis("Mouse X") * 2;
        mouseY += Input.GetAxis("Mouse Y") * 2;
    }
    
    //Follow Cam
    void FollowCam()
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
            return;
        }

        //Camera Rotation
        cameraArmTr.rotation = Quaternion.Euler(-mouseY, mouseX, 0);
    }

    //Zoom Cam
    void CameraSpring()
    {
        dir = transform.position- playerTr.position;
        dis = Vector3.Distance(playerTr.position, this.transform.position);

        if (Physics.Raycast(playerTr.position+ (new Vector3(0, 1f, -0.2f)), dir, out hit, dis))
        {
            if (hit.transform.gameObject != this.gameObject&&playerTr.gameObject != hit.transform.gameObject)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 2f, hit.point.normalized.z),Time.fixedDeltaTime);
                Camera.main.nearClipPlane = Mathf.Lerp(Camera.main.nearClipPlane, 3.5f, Time.fixedDeltaTime);
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 4f, -3f), Time.fixedDeltaTime);
            Camera.main.nearClipPlane = Mathf.Lerp(Camera.main.nearClipPlane, 0.3f, Time.fixedDeltaTime);
        }

    }


}
