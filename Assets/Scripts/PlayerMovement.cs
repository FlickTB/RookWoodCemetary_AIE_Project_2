using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]                                                                                                    //Title for usage in Unity
    public float moveSpeed;                                                                                                 //Variable for the movement speed

    public float groundDrag;                                                                                                //Variable for how much the ground slows down the player

    public float jumpForce;                                                                                                 //Variable for how high the player jumps
    public float jumpCooldown;                                                                                              //Variable for how often the player can jump
    public float airMultiplier;                                                                                             //Variable for how being in the air affects movement speed
    bool readyToJump;                                                                                                       //Variable for when the player is able to jump

    [Header("Keydinds")]                                                                                                    //Title for usage in Unity
    public KeyCode jumpKey = KeyCode.Space;                                                                                 //The input to jump

    [Header("Ground Check")]                                                                                                //Title for usage in Unity
    public float playerHeight;                                                                                              //Variable for how tall the player is
    public LayerMask whatIsGround;                                                                                          //A reference for what is considered the ground
    bool grounded;                                                                                                          //Variable for the player being on the ground

    public Transform orientation;                                                                                           //A reference for the direction the player is facing

    float horizontalInput;                                                                                                  //Variable for the player's movement
    float verticalInput;                                                                                                    //Variable for the player's move

    Vector3 moveDirection;                                                                                                  //Variable for the direction the player is moving

    Rigidbody rb;                                                                                                           //A reference for physics being used on the player

    private void Start()                                                                                                    //Function called on the first frame
    {
        rb = GetComponent<Rigidbody>();                                                                                     //Calling on getting the rigidbody properties
        rb.freezeRotation = true;                                                                                           //Variable for stopping rotation
        readyToJump = true;                                                                                                 //Variable for allowing the player to jump
    }

    private void Update()                                                                                                   //Funtion called on every frame
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);             //Variable for determing the grounded properties

        MyInput();                                                                                                          //Calling on another function
        SpeedControl();                                                                                                     //Calling on another function

        if(grounded)                                                                                                        //Runs when the criteria is met
        {
            rb.drag = groundDrag;                                                                                           //Determines what drag is
        }    
        else                                                                                                                //If criteria isn't met, do this insted
        {
            rb.drag = 0;                                                                                                    //Determines what drag is
        }
    }

    private void FixedUpdate()                                                                                              //Function is called on every set amount of frames
    {
        MovePlayer();                                                                                                       //Calling on another function
    }

    void MyInput()                                                                                                          //Function for player's controls
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");                                                                   //Variable for getting the horizontal movement
        verticalInput = Input.GetAxisRaw("Vertical");                                                                       //Variable for getting the vertical movement

        if (Input.GetKey(jumpKey) && readyToJump && grounded)                                                               //Runs when the criteria is met
        {
            readyToJump = false;                                                                                            //The player can no longer jump
            Jump();                                                                                                         //Calling on another function
            Invoke(nameof(ResetJump), jumpCooldown);                                                                        //Calling on another function
        }
    }

    void MovePlayer()                                                                                                       //Function for player's movement
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;                          //Determines how the player moves foward 

        if (grounded)                                                                                                       //Runs when the criteria is met
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);                                       //Allows speed to increase
        }

        else if(!grounded)                                                                                                  //Runs when the criteria is met
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);                       //Allows speed to increase, but adjusted for air movement
        }
    }

    void SpeedControl()                                                                                                     //Function for setting the maximum speed of the player
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                    //Creates a variable for the maximum speed

        if(flatVel.magnitude > moveSpeed)                                                                                   //Runs when the criteria is met
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;                                                            //Defines the variable for the maximum speed
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);                                           //Calls on variable for the maximum speed
        }
    }

    void Jump()                                                                                                             //Function for jumping
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);                                                        //Creates velocity
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);                                                           //Adds force to the jump
    }

    void ResetJump()                                                                                                        //Function for resetting the jump
    {
        readyToJump = transform;                                                                                            //Allows the player to jump again
    }
}