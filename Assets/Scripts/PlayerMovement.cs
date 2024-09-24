using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]                                                                                                    //Title for usage in Unity
    float moveSpeed;                                                                                                        //Variable for the movement speed
    public float walkSpeed;                                                                                                 //Variable for the walk speed
    public float sprintSpeed;                                                                                               //Variable for the sprint speed
    public float wallrunSpeed;

    public float dashSpeed;
    public float dashSpeedChangeFactor;

    public float maxYSpeed;

    public float groundDrag;                                                                                                //Variable for how much the ground slows down the player

    [Header("Jumping")]                                                                                                     //Title for usage in Unity
    public float jumpForce;                                                                                                 //Variable for how high the player jumps
    public float jumpCooldown;                                                                                              //Variable for how often the player can jump
    public float airMultiplier;                                                                                             //Variable for how being in the air affects movement speed
    bool readyToJump;                                                                                                       //Variable for when the player is able to jump

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
    bool grounded;                                                                                                          //Variable for the player being on the ground

    [Header("Slope Handling")]                                                                                              //Title for usage in Unity
    public float maxSlopeAngle;                                                                                             //Variable for the angles the player can't climb
    RaycastHit slopeHit;                                                                                                    //Stores that the player is on a slope
    bool exitingSlope;                                                                                                      //Variable for knowing when the player isn't on a slope

    public Transform orientation;                                                                                           //A reference for the direction the player is facing

    float horizontalInput;                                                                                                  //Variable for the player's movement
    float verticalInput;                                                                                                    //Variable for the player's movement

    Vector3 moveDirection;                                                                                                  //Variable for the direction the player is moving

    Rigidbody rb;                                                                                                           //A reference for physics being used on the player

    public MovementState state;                                                                                             //A list of movement options the player can perform

    public enum MovementState                                                                                               //The list of movement options
    {
        freeze,
        walking,                                                                                                            //Variable for the walk state
        sprinting,                                                                                                          //Variable for the sprint state
        wallrunning,
        crouching,                                                                                                          //Variable for the crouching state
        dashing,
        air                                                                                                                 //Variable for the air state
    }

    public bool freeze;
    public bool activeGrapple;
    public bool dashing;
    public bool wallrunning;

    void Start()                                                                                                    //Function called on the first frame
    {
        rb = GetComponent<Rigidbody>();                                                                                     //Calling on getting the rigidbody properties
        rb.freezeRotation = true;                                                                                           //Variable for stopping rotation
        readyToJump = true;                                                                                                 //Variable for allowing the player to jump
        startYScale = transform.localScale.y;                                                                               //Records the starting height of the player
    }

    void Update()                                                                                                   //Funtion called on every frame
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

    void FixedUpdate()                                                                                              //Function is called on every set amount of frames
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
        if(Input.GetKeyDown(crouchKey))                                                                                     //When the crouch button is pressed
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);               //Changes the height of the player, thus they crouch
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);                                                              //Applies force to the player so they stay grounded
        }
        if(Input.GetKeyUp(crouchKey))                                                                                       //When the crouch button is released
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);                //Reverts the player height back to normal
        }
    }

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;
    MovementState lastState;
    bool keepMomentum;

    void StateHandler()                                                                                                     //Function that manages the different movement states
    {
        if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }
        else if (freeze)
        {
            state = MovementState.freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        else if(dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            speedChangeFactor = dashSpeedChangeFactor;
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
            moveSpeed = walkSpeed;
        }
        else                                                                                                                //Otherwise the player is in the air
        {
            state = MovementState.air;                                                                                      //Changes the movement state to air
            
            if(desiredMoveSpeed < sprintSpeed)
            {
                desiredMoveSpeed = walkSpeed;
            }
            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == MovementState.dashing)
        {
            keepMomentum = true;
        }

        if(desiredMoveSpeedHasChanged)
        {
            if(keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }
        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    float speedChangeFactor;

    IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);
            time += Time.deltaTime * boostFactor;
            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    void MovePlayer()                                                                                                       //Function for player's movement
    {
        if(activeGrapple)
        {
            return;
        }
        if(state == MovementState.dashing)
        {
            return;
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

        if (!wallrunning)
        {
            rb.useGravity = !OnSlope();                                                                                     //Disables gravity on the slope to stop the player moving when idle
        }
    }

    void SpeedControl()                                                                                                     //Function for setting the maximum speed of the player
    {
        if(activeGrapple)
        {
            return;
        }
        if(OnSlope() && !exitingSlope)                                                                                      //If they are on a slope
        {
            if(rb.velocity.magnitude > moveSpeed)                                                                           //If they are going faster than the movement speed
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;                                                           //Slow down the player to the correct speed
            }
        }
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                //Creates a variable for the maximum speed

            if (flatVel.magnitude > moveSpeed)                                                                              //If they are going faster than the movement speed
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;                                                        //Defines the variable for the maximum speed
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);                                       //Calls on variable for the maximum speed
            }
        }
        if(maxYSpeed != 0 && rb.velocity.y > maxYSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x, maxYSpeed, rb.velocity.z);
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
        exitingSlope = false;                                                                                               //On a slope
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    Vector3 velocityToSet;
    
    void SetVelocity()
    {
        rb.velocity = velocityToSet;
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

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y = startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
                           + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;

    }
}