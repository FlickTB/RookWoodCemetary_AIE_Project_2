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
    public bool useCameraForward = true;                                                                                    //
    public bool allowAllDirections = true;                                                                                  //
    public bool disableGravity = false;                                                                                     //

    [Header("Cooldown")]                                                                                                    //Title for usage in Unity
    public float dashCd;                                                                                                    //Time for dash to reset
    float dashCdTimer;                                                                                                      //

    [Header("Input")]                                                                                                       //Title for usage in Unity
    public KeyCode dashKey = KeyCode.E;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(dashKey))
        {
            Dash();
        }
        if(dashCdTimer > 0)
        {
            dashCdTimer -= Time.deltaTime;
        }
    }
    void Dash()
    {
        if (dashCdTimer > 0)
        {
            return;
        }
        else
        {
            dashCdTimer = dashCd;
        }

        pm.dashing = true;
        pm.maxYSpeed = maxDashYSpeed;

        cam.DoFov(dashFov);

        Transform forwardT;

        if(useCameraForward)
        {
            forwardT = playerCam;
        }
        else
        {
            forwardT = orientation;
        }

        Vector3 direction = GetDirection(forwardT);
        Vector3 forceToApply = direction * dashForce + orientation.up * dashUpwardForce;

        if(disableGravity)
        {
            rb.useGravity = false;
        }

        rb.AddForce(forceToApply, ForceMode.Impulse);
        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(ResetDash), dashDuration);
    }

    Vector3 delayedForceToApply;

    void DelayedDashForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    void ResetDash()
    {
        pm.dashing = false;
        pm.maxYSpeed = 0;

        cam.DoFov(85f);

        if (disableGravity)
        {
            rb.useGravity = true;
        }
    }

    Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3();

        if(allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        }
        if(verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }

}