using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dashing : MonoBehaviour
{
    [Header("References")]                                                                                                  //Title for usage in Unity
    public Transform orientation;                                                                                           //Reference for camera orientation
    public Transform playerCam;                                                                                             //Reference for player camera
    Rigidbody rb;                                                                                                           //Reference for the player physics
    PlayerMovement pm;                                                                                                      //Refernece for the player movement script

    [Header("Dahing")]                                                                                                      //Title for usage in Unity
    public float dashForce;                                                                                                 //A varible for the horizontal dash speed
    public float dashUpwardForce;                                                                                           //A varible for the vertical dash speed
    public float maxDashYSpeed;                                                                                             //A varible for the max amount of vertical speed
    public float dashDuration;                                                                                              //A varible for how long the dash last

    [Header("CameraEffects")]                                                                                               //Title for usage in Unity
    public PlayerCam cam;                                                                                                   //Reference for player camera
    public float dashFov;                                                                                                   //Variable for the change to FOV during dash

    [Header("Settings")]                                                                                                    //Title for usage in Unity
    public bool useCameraForward = true;                                                                                    //Variable for camera's orientation
    public bool allowAllDirections = true;                                                                                  //Variable for camera's directions
    public bool disableGravity = false;                                                                                     //Varible for gravity usage

    [Header("Cooldown")]                                                                                                    //Title for usage in Unity
    public float dashCd;                                                                                                    //Time for dash to reset
    float dashCdTimer;                                                                                                      //Time until dash cooldown

    [Header("Input")]                                                                                                       //Title for usage in Unity
    public KeyCode dashKey = KeyCode.E;                                                                                     //Input for dash

    void Start()                                                                                                            //Function called at the start
    {
        rb = GetComponent<Rigidbody>();                                                                                     //Refernce to the rigidbody
        pm = GetComponent<PlayerMovement>();                                                                                //Refernce to player movement script
    }

    void Update()                                                                                                           //Function called every frame
    {
        if (Input.GetKeyDown(dashKey))                                                                                      //If the player dashes
        {
            Dash();                                                                                                         //Call the dash script
        }
        if (dashCdTimer > 0)                                                                                                //If the cooldown timer is greater than zero
        {
            dashCdTimer -= Time.deltaTime;                                                                                  //Cooldown timer ticks down
        }
    }
    void Dash()                                                                                                             //Function for the player dashing
    {
        if (dashCdTimer > 0)                                                                                                //If the dash timer is greater than 0
        {
            return;                                                                                                         //Return
        }
        else                                                                                                                //Otherwise
        {
            dashCdTimer = dashCd;                                                                                           //The dash timer is reset
        }

        pm.dashing = true;                                                                                                  //The player is dashing
        pm.maxYSpeed = maxDashYSpeed;                                                                                       //The dash vertical speed 

        cam.DoFov(dashFov);                                                                                                 //Calls the FOV change function

        Transform forwardT;                                                                                                 //Refernce for forward direction

        if (useCameraForward)                                                                                               //If the camera faces forward
        {
            forwardT = playerCam;                                                                                           //The forward direction is the direction of the camera
        }
        else                                                                                                                //Otherwise
        {
            forwardT = orientation;                                                                                         //The direction is the camera orientation
        }

        Vector3 direction = GetDirection(forwardT);                                                                         //New vector for direction
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;                                    //New vector for dash force

        if (disableGravity)                                                                                                 //If the gravity is disabled
        {
            rb.useGravity = false;                                                                                          //Don't use gravity
        }

        rb.AddForce(forceToApply, ForceMode.Impulse);                                                                       //Add force
        Invoke(nameof(DelayedDashForce), 0.025f);                                                                           //Delay the dash

        Invoke(nameof(ResetDash), dashDuration);                                                                            //Reset the dash
    }

    Vector3 delayedForceToApply;                                                                                            //Vector for the delayed force

    void DelayedDashForce()                                                                                                 //Function for the delayed dash force
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);                                                                //Add force
    }

    void ResetDash()                                                                                                        //Function for the dash reset
    {
        pm.dashing = false;                                                                                                 //Player isn't dashing
        pm.maxYSpeed = 0;                                                                                                   //Dash vertical speed is set to zero

        cam.DoFov(85f);                                                                                                     //Change the FOV of the camera

        if (disableGravity)                                                                                                 //If the gravity is disabled
        {
            rb.useGravity = true;                                                                                           //The player uses gavity again
        }
    }

    Vector3 GetDirection(Transform forwardT)                                                                                //Vector for the player direction
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");                                                             //Get the horizontal movement
        float verticalInput = Input.GetAxisRaw("Vertical");                                                                 //Get the vertical movement
        Vector3 direction = new Vector3();                                                                                  //Make a new vector direction

        if (allowAllDirections)                                                                                             //If the player can dash any direction
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;                                //The player dashes in the direction they're moving
        }
        else                                                                                                                //Otherwise
        {
            direction = forwardT.forward;                                                                                   //Dash forward
        }
        if (verticalInput == 0 && horizontalInput == 0)                                                                     //If the player inputs are zero
        {
            direction = forwardT.forward;                                                                                   //Dash forward
        }

        return direction.normalized;                                                                                        //Return the direction
    }

}