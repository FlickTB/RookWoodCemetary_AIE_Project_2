using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]                                                                                                    //Title for usage in Unity
    float moveSpeed;                                                                                                        //Variable for the movement speed
    public float walkSpeed;                                                                                                 //Variable for the walk speed
    public float sprintSpeed;                                                                                               //Variable for the sprint speed
    public float wallrunSpeed;                                                                                              //Variable for the wall run speed
    public float dashSpeed;                                                                                                 //Variable for dash speed
    public float dashSpeedChangeFactor;                                                                                     //Variable for how quickly the dash take effect
    public float maxYSpeed;                                                                                                 //Varible that limits how quickly the player moves vertically
    public float groundDrag;                                                                                                //Variable for how much the ground slows down the player

    [Header("Jumping")]                                                                                                     //Title for usage in Unity
    public float jumpForce;                                                                                                 //Variable for how high the player jumps
    public float jumpForceNormal;                                                                                           //Variable for what the standard jump force is
    public float jumpCooldown;                                                                                              //Variable for how often the player can jump
    public float airMultiplier;                                                                                             //Variable for how being in the air affects movement speed
    bool readyToJump;                                                                                                       //Variable for when the player is able to jump
    public float leapMultiplier;                                                                                            //Variable for how much additional force is applied for leaping

    [Header("Crouching")]                                                                                                   //Title for usage in Unity
    public float crouchSpeed;                                                                                               //Variable for crouch speed
    public float crouchYScale;                                                                                              //Variable for performing the crouch
    float startYScale;                                                                                                      //Variable for storing the starting player height

    [Header("Keydinds")]                                                                                                    //Title for usage in Unity
    public KeyCode jumpKey = KeyCode.Space;                                                                                 //The input to jump
    public KeyCode sprintKey = KeyCode.LeftShift;                                                                           //The input to sprint
    public KeyCode crouchKey = KeyCode.LeftControl;                                                                         //The input to crouch

    [Header("Ground Check")]                                                                                                //Title for usage in Unity
    public float playerHeight;                                                                                              //Variable for how tall the player is
    public LayerMask whatIsGround;                                                                                          //A reference for what is considered the ground
    public bool grounded;                                                                                                   //Variable for the player being on the ground

    [Header("Slope Handling")]                                                                                              //Title for usage in Unity
    public float maxSlopeAngle;                                                                                             //Variable for the angles the player can't climb
    RaycastHit slopeHit;                                                                                                    //Stores that the player is on a slope
    bool exitingSlope;                                                                                                      //Variable for knowing when the player isn't on a slope

    [Header("Other")]                                                                                                       //Title for usage in Unity
    public Transform orientation;                                                                                           //A reference for the direction the player is facing
    float horizontalInput;                                                                                                  //Variable for the player's movement
    float verticalInput;                                                                                                    //Variable for the player's movement
    Vector3 moveDirection;                                                                                                  //Variable for the direction the player is moving
    Rigidbody rb;                                                                                                           //A reference for physics being used on the player
    public MovementState state;                                                                                             //A list of movement options the player can perform

    public enum MovementState                                                                                               //The list of movement options
    {
        freeze,                                                                                                             //Varaible for the freeze state, used during grapple
        walking,                                                                                                            //Variable for the walk state
        sprinting,                                                                                                          //Variable for the sprint state
        wallrunning,                                                                                                        //Variable for the wall running state
        crouching,                                                                                                          //Variable for the crouching state
        dashing,                                                                                                            //Variable for the dashing state
        air                                                                                                                 //Variable for the air state
    }

    public bool freeze;                                                                                                     //Variable for the freeze state, used during the grapple
    public bool activeGrapple;                                                                                              //Variable for the grapple state
    public bool dashing;                                                                                                    //Variable for the dashing state
    public bool wallrunning;                                                                                                //Variable for the wall running state

    void Start()                                                                                                            //Function called on the first frame
    {
        rb = GetComponent<Rigidbody>();                                                                                     //Calling on getting the rigidbody properties
        rb.freezeRotation = true;                                                                                           //Variable for stopping rotation
        readyToJump = true;                                                                                                 //Variable for allowing the player to jump
        startYScale = transform.localScale.y;                                                                               //Records the starting height of the player
    }

    void Update()                                                                                                           //Funtion called on every frame
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);             //Variable for determing the grounded properties

        MyInput();                                                                                                          //Calling on another function
        SpeedControl();                                                                                                     //Calling on another function
        StateHandler();                                                                                                     //Calling on another function

        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching)         //Checks the player is on the ground
        {
            rb.drag = groundDrag;                                                                                           //Determines what drag is
        }    
        else                                                                                                                //If criteria isn't met, do this insted
        {
            rb.drag = 0;                                                                                                    //Determines what drag is
        }
    }

    void FixedUpdate()                                                                                                      //Function is called on every set amount of frames
    {
        MovePlayer();                                                                                                       //Calling on another function
    }

    void MyInput()                                                                                                          //Function for player's controls
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");                                                                   //Variable for getting the horizontal movement
        verticalInput = Input.GetAxisRaw("Vertical");                                                                       //Variable for getting the vertical movement

        if (Input.GetKey(jumpKey) && readyToJump && grounded)                                                               //When the player tries to jump
        {
            readyToJump = false;                                                                                            //The player can no longer jump
            Jump();                                                                                                         //Calling on another function
            Invoke(nameof(ResetJump), jumpCooldown);                                                                        //Calling on another function
        }
        if (Input.GetKey(KeyCode.Q) && readyToJump && grounded)                                                             //When the player wants to leap                  
        {
            readyToJump = false;                                                                                            //The player can no longer jump
            jumpForce = jumpForce * leapMultiplier;                                                                         //Change the jump to a leap
            Jump();                                                                                                         //Calling on another function
            Invoke(nameof(ResetJump), jumpCooldown);                                                                        //Calling on another function
        }
        if (Input.GetKeyDown(crouchKey))                                                                                    //When the crouch button is pressed
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);               //Changes the height of the player, thus they crouch
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);                                                              //Applies force to the player so they stay grounded
        }
        if (Input.GetKeyUp(crouchKey))                                                                                      //When the crouch button is released
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);                //Reverts the player height back to normal
        }
    }

    float desiredMoveSpeed;                                                                                                 //Variable for the speed the player should be
    float lastDesiredMoveSpeed;                                                                                             //Variable for what speed the player used to be
    MovementState lastState;                                                                                                //Variable for the movement state the player used to be, used to keep momentum
    bool keepMomentum;                                                                                                      //Variable for if the player keeps momentum

    void StateHandler()                                                                                                     //Function that manages the different movement states
    {
        if (wallrunning)                                                                                                    //If the player is wall running
        {
            state = MovementState.wallrunning;                                                                              //Changes the movement state to wall running
            desiredMoveSpeed = wallrunSpeed;                                                                                //Sets the player speed to wall running speed
        }
        else if (freeze)                                                                                                    //If the player is in the freeze state (grappling)
        {
            state = MovementState.freeze;                                                                                   //Changes the movement state to freeze
            moveSpeed = 0;                                                                                                  //Sets move speed to zero
            rb.velocity = Vector3.zero;                                                                                     //Sets velocity to zero
        }
        else if(dashing)                                                                                                    //If the player wants to dash
        {
            state = MovementState.dashing;                                                                                  //Changes the movement state to dashing
            desiredMoveSpeed = dashSpeed;                                                                                   //Changes the player speed to dashing
            speedChangeFactor = dashSpeedChangeFactor;                                                                      //Changes how quicly the player alters their speed
        }
        else if(Input.GetKey(crouchKey))                                                                                    //If the player wants to crouch
        {
            state = MovementState.crouching;                                                                                //Changes the movement state to crouching
            desiredMoveSpeed = crouchSpeed;                                                                                 //Changes the movement speed to crouching
        }
        else if(grounded && Input.GetKey(sprintKey))                                                                        //If the player wants to sprint
        {
            state = MovementState.sprinting;                                                                                //Changes the movement state to sprinting
            desiredMoveSpeed = sprintSpeed;                                                                                 //Changes the movement speed to sprinting
        }
        else if(grounded)                                                                                                   //Otherwise the player is walking when on the ground
        {
            state = MovementState.walking;                                                                                  //Changes the movement state to walking
            desiredMoveSpeed = walkSpeed;                                                                                   //Changes the movement speed to walking
            moveSpeed = walkSpeed;                                                                                          //Failsafe to ensures the player can walk again
        }
        else                                                                                                                //Otherwise the player is in the air
        {
            state = MovementState.air;                                                                                      //Changes the movement state to air
            
            if(desiredMoveSpeed < sprintSpeed)                                                                              //If the player's speed is slower than a sprint                                                                        
            {
                desiredMoveSpeed = walkSpeed;                                                                               //The player's speed is set to walk
            }
            else                                                                                                            //Otherwise the speed changes
            {
                desiredMoveSpeed = sprintSpeed;                                                                             //Player's speed becomes a sprint speed
            }
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;                                         //Variable for when the player speed changes

        if(lastState == MovementState.dashing)                                                                              //If the last movement state was dashing
        {
            keepMomentum = true;                                                                                            //Keep the player momentum
        }

        if(desiredMoveSpeedHasChanged)                                                                                      //If the player speed has changed
        {
            if(keepMomentum)                                                                                                //If the player keeps their momentum
            {
                StopAllCoroutines();                                                                                        //Calls another function
                StartCoroutine(SmoothlyLerpMoveSpeed());                                                                    //Calls another function
            }
            else                                                                                                            //Otherwise
            {
                StopAllCoroutines();                                                                                        //Calls another function
                moveSpeed = desiredMoveSpeed;                                                                               //Movement becomes the whaat it should be
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;                                                                            //The last move speed becomes the current move speed
        lastState = state;                                                                                                  //The last state is the current state
    }

    float speedChangeFactor;                                                                                                //Variable for the rate at which the player changes speed

    IEnumerator SmoothlyLerpMoveSpeed()                                                                                     //Maths function to change speed
    {
        float time = 0;                                                                                                     //Variable for time
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);                                                         //Variable for the difference between the current speed and the intended speed
        float startValue = moveSpeed;                                                                                       //The start calculation speed is the current speed

        float boostFactor = speedChangeFactor;                                                                              //The boost change is the same as the speed change

        while (time < difference)                                                                                           //While the time is smaller the difference
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);                                        //The move speed gets calculated 
            time += Time.deltaTime * boostFactor;                                                                           //The time value changes
            yield return null;                                                                                              //Returns null
        }

        moveSpeed = desiredMoveSpeed;                                                                                       //The move speed becomes a the desired move speed
        speedChangeFactor = 1f;                                                                                             //The speed change gets a new value
        keepMomentum = false;                                                                                               //Player doesn't keep their momentum
    }

    void MovePlayer()                                                                                                       //Function for player's movement
    {
        if(activeGrapple)                                                                                                   //If the player is grappling
        {
            return;                                                                                                         //Returns
        }
        if(state == MovementState.dashing)                                                                                  //If the player is dashing
        {
            return;                                                                                                         //Returns
        }

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;                          //Determines how the player moves foward 

        if(OnSlope() && !exitingSlope)                                                                                      //When the player is on a slope
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);                                        //Add force parallel to the slope

            if (rb.velocity.y > 0)                                                                                          //If they are moving up
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);                                                           //Add downward force to limit speed
            }
        }
        if(grounded)                                                                                                        //If they are on the ground
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);                                       //Allows speed to increase
        }
        else if(!grounded)                                                                                                  //If they are not grounded
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);                       //Allows speed to increase, but adjusted for air movement
        }

        if (!wallrunning)                                                                                                   //If the player isn't wall running
        {
            rb.useGravity = !OnSlope();                                                                                     //Disables gravity on the slope to stop the player moving when idle
        }
    }

    void SpeedControl()                                                                                                     //Function for setting the maximum speed of the player
    {
        if(activeGrapple)                                                                                                   //If the player is grappling
        {
            return;                                                                                                         //Returns
        }
        if(OnSlope() && !exitingSlope)                                                                                      //If they are on a slope
        {
            if(rb.velocity.magnitude > moveSpeed)                                                                           //If they are going faster than the movement speed
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;                                                           //Slow down the player to the correct speed
            }
        }
        else                                                                                                                //Otherwise
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                //Creates a variable for the maximum speed

            if (flatVel.magnitude > moveSpeed)                                                                              //If they are going faster than the movement speed
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;                                                        //Defines the variable for the maximum speed
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);                                       //Calls on variable for the maximum speed
            }
        }
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)                                                                     //If the max y speed is greater than zero and has velocity
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);                                             //The velocity gets a new value
        }
    }

    void Jump()                                                                                                             //Function for jumping
    {
        exitingSlope = true;                                                                                                //Not a slope
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                        //Creates velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);                                                           //Adds force to the jump
    }

    void ResetJump()                                                                                                        //Function for resetting the jump
    {
        readyToJump = transform;                                                                                            //Allows the player to jump again
        jumpForce = jumpForceNormal;                                                                                        //Resets the jump force back to the standard value
        exitingSlope = false;                                                                                               //On a slope
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)                                              //Function for grappling to a location
    {
        activeGrapple = true;                                                                                               //The player is grappling
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);                        //Calculates the velocity for the grapple
        Invoke(nameof(SetVelocity), 0.1f);                                                                                  //Calls another function
    }

    Vector3 velocityToSet;                                                                                                  //Variable for a velocity for the player
    
    void SetVelocity()                                                                                                      //Function for the player's velocity
    {
        rb.velocity = velocityToSet;                                                                                        //Current velocity becomes the intended velocity
    }

    bool OnSlope()                                                                                                          //Function for being on a slope
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))                     //If they are on a slope
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);                                                       //Change the movement speed to stay consistent
            return angle < maxSlopeAngle && angle != 0;                                                                     //As long as they are on a intended surface
        }

        return false;                                                                                                       //Safety stop
    }

    Vector3 GetSlopeMoveDirection()                                                                                         //Function that gets the direction the player is moveing on a slope
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;                                           //Keeps the player moving correctly
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)                      //Function for the player's jump velocity
    {
        float gravity = Physics.gravity.y;                                                                                  //The variable gravity is equal to the game's gravity
        float displacementY = endPoint.y = startPoint.y;                                                                    //The displacement is the same as the start and end points
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);                     //Calculation for new Vector

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);                                       //Calculation for the vertical velocity
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)                                  //Calculation for the horizontal velocity
                           + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));                                 //Calculation for the horizontal velocity

        return velocityXZ + velocityY;                                                                                      //Returns the velocities

    }
}