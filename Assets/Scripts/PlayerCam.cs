using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerCam : MonoBehaviour
{
    public float sensX;                                                                                                     //A varible for mouse sensitivity
    public float sensY;                                                                                                     //A varible for mouse sensitivity

    public Transform orientation;                                                                                           //A refernce for where the player is supposed to be looking
    public Transform camHolder;

    float xRotation;                                                                                                        //A varible for the camera movement
    float yRotation;                                                                                                        //A varible for the camera movement

    void Start()                                                                                                            //Function is called at the start of the scene
    {
        Cursor.lockState = CursorLockMode.Locked;                                                                           //Locks the camera to follow the mouse
        Cursor.visible = false;                                                                                             //Hides the cursor
    }

    void Update()                                                                                                           //Function is called every frame
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;                                                //Get mouse inputs for the X axis for a period of time
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;                                                //Get mouse inputs for the Y axis for a period of time

        yRotation += mouseX;                                                                                                //Sets the variable to align with mouse movement
        xRotation -= mouseY;                                                                                                //Sets the variable to align with mouse movement
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);                                                                      //Limits how far the player can look up & down

        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);                                                     //Rotates the camera on the X axis
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);                                                           //Rotates the camera on the Y axis
    }

    public void DoFov(float endValue)
    {
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}