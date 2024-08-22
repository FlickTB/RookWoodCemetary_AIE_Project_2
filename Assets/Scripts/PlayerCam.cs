using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;                                                                           //Locks the camera to follow the mouse
        Cursor.visible = false;                                                                                             //Hides the cursor
    }

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;                                                //Get mouse inputs for the X axis
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;                                                //Get mouse inputs for the Y axis

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);                                                                      //Limits how far the player can look up & down

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);                                                     //Rotates the camera on the X axis
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);                                                           //Rotates the camera on the Y axis
    }
}