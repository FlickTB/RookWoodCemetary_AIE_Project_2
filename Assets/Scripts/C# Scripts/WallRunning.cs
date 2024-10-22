using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]                                                                                                 //Title for Unity
    public LayerMask whatIsWall;                                                                                            //Layer for wall
    public LayerMask whatIsGround;                                                                                          //Layer for ground
    public float wallRunForce;                                                                                              //Variable for wall run speed
    public float wallJumpUpForce;                                                                                           //Variable for vertical jump
    public float wallJumpSideForce;                                                                                         //Variable for horizontal jump
    public float maxWallRunTime;                                                                                            //Variable for max wall run time
    float wallRunTimer;                                                                                                     //Variable for wall run timer

    [Header("Input")]                                                                                                       //Title for Unity
    public KeyCode jumpKey = KeyCode.Space;                                                                                 //Input for jump
    float horizontalInput;                                                                                                  //Variable for horizontal input
    float verticalInput;                                                                                                    //Variable for vertical input

    [Header("Detection")]                                                                                                   //Title for Unity
    public float wallCheckDistance;                                                                                         //Variable for distance from wall
    public float minJumpHeight;                                                                                             //Variable for minimum jump
    RaycastHit leftWallHit;                                                                                                 //Variable for hitting left wall
    RaycastHit rightWallHit;                                                                                                //Variable for hitting right wall
    bool wallLeft;                                                                                                          //Variable for left wall
    bool wallRight;                                                                                                         //Variable for right wall

    [Header("Exiting")]                                                                                                     //Title for Unity
    bool exitingWall;                                                                                                       //Variable for leaving the wall
    public float exitWallTime;                                                                                              //Variable for grace period between wall jumps
    float exitWallTimer;                                                                                                    //Variable for the timer for the grace period

    [Header("Gravity")]                                                                                                     //Title for Unity
    public bool useGravity;                                                                                                 //Variable for using gravity
    public float gravityCounterForce;                                                                                       //Variable for the gravity counter force

    [Header("References")]                                                                                                  //Title for Unity
    public Transform orientation;                                                                                           //Reference for camera orientation
    public PlayerCam cam;                                                                                                   //Reference for player camera
    PlayerMovement pm;                                                                                                      //Refernece for the player movement script
    Rigidbody rb;                                                                                                           //Reference for the player physics
    
    void Start()                                                                                                            //Function called at the start
    {
        rb = GetComponent<Rigidbody>();                                                                                     //Gets the rigidbody
        pm = GetComponent<PlayerMovement>();                                                                                //Gets the player movement script
    }

    void Update()                                                                                                           //Function called every frame
    {
        CheckForWall();                                                                                                     //Calls other function
        StateMachine();                                                                                                     //Calls other function
    }

    void FixedUpdate()                                                                                                      //Function called at the end of every frame
    {
        if (pm.wallrunning)                                                                                                 //If the player is wall running
        {
            WallRunningMovement();                                                                                          //Calls the other script
        }
    }

    void CheckForWall()                                                                                                     //Function for if the player is on the wall
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);//Calculation if the player is on the right wall
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall); //Calculation if the player is on the left wall
    }

    bool AboveGround()                                                                                                      //Function for if the player is above the ground
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);                             //Returns the poistion from the floor
    }

    void StateMachine()                                                                                                     //Function for the state the player is in for wall running
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");                                                                   //Gets the player's horizontal input
        verticalInput = Input.GetAxisRaw("Vertical");                                                                       //Gets the player's vertical input

        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)                                   //If the player is is on a wall, off the floor and has left a wall
        {
            if (!pm.wallrunning)                                                                                            //If the player movement isn't using wall running
            {
                StartWallRun();                                                                                             //Calls another function
            }
            if (wallRunTimer > 0)                                                                                           //If the wall run timer is greater than zero
            {
                wallRunTimer -= Time.deltaTime;                                                                             //Countdown the timer
            }
            if (wallRunTimer <= 0 && pm.wallrunning)                                                                        //If the timer exipres and the player is on the wall
            {
                exitingWall = true;                                                                                         //Player stops the wall run
                exitWallTimer = exitWallTime;                                                                               //The exit wall timer starts
            }
            if (Input.GetKeyDown(jumpKey))                                                                                  //If the player jumps
            {
                WallJump();                                                                                                 //Calls the other function
            }
        }
        else if (exitingWall)                                                                                               //Otherwise if the player is exiting the wall
        {
            if (pm.wallrunning)                                                                                             //If they are wall running
            {
                StopWallRun();                                                                                              //Calls the function
            }
            if (exitWallTimer > 0)                                                                                          //If the exit wall time if greater than zero
            {
                exitWallTimer -= Time.deltaTime;                                                                            //The exit wall time counts down

                if (wallLeft || wallRight)                                                                                  //If the player is on a wall
                {
                    exitWallTimer = exitWallTime;                                                                           //The leave wall timer is reset
                }
            }
            if (exitWallTimer <= 0)                                                                                         //If the exit wall timer expired
            {
                exitingWall = false;                                                                                        //The player is not exiting the wall
            }
        }
        else                                                                                                                //Otherwise
        {
            if(pm.wallrunning)                                                                                              //If the player is wall running in the player movement
            {
                StopWallRun();                                                                                              //Calls another function
            }
        }
    }

    void StartWallRun()                                                                                                     //Function for the player wall running
    {
        pm.wallrunning = true;                                                                                              //The player is wall running in the player movement
        wallRunTimer = maxWallRunTime;                                                                                      //The wall run timer is reset
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                        //New velocity is set

        if (wallLeft)                                                                                                       //If the player is on the left wall
        {
            cam.DoTilt(-5f);                                                                                                //Make the camera tilt
        }
        if (wallRight)                                                                                                      //If the player is on the right wall
        {
            cam.DoTilt(5f);                                                                                                 //Make the camera tilt
        }
    }

    void WallRunningMovement()                                                                                              //Function for wall running
    {
        rb.useGravity = useGravity;                                                                                         //Turns on the player gravity

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;                                          //Vector for when they are on the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);                                                      //Vector for when moving on the wall

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)                 //If the player is changing directions when wall running
        {
            wallForward = -wallForward;                                                                                     //The player will move base on their direction
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);                                                           //Adds force during the wall run

        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))                                      //If the player is on the wall
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);                                                                //Add force to keep the on the wall
        }
        if (useGravity)                                                                                                     //If the player is using gravity
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);                                               //Add force to reduce the effects of gravity
        }
    }

    void StopWallRun()                                                                                                      //Function for the wall run ending
    {
        pm.wallrunning = false;                                                                                             //The player stops wall running
        cam.DoTilt(0f);                                                                                                     //The camera oreintation resets
    }

    void WallJump()                                                                                                         //Function for the wall jumping
    {
        exitingWall = true;                                                                                                 //The player starts exiting the wall
        exitWallTimer = exitWallTime;                                                                                       //The exit wall time starts counting down

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;                                          //Creates yje vector for being on the wall
        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;                             //Creates the vector for what force is needed

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                        //Makes a new velocity
        rb.AddForce(forceToApply, ForceMode.Impulse);                                                                       //Add force
    }
}