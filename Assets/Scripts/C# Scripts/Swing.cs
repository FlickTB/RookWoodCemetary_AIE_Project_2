using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField] private Animator animator;                                                                             //Refernce for the animation
    private bool canSecondSwing = false;

    void Update()                                                                                                           //Function called every frame
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))                                                                               //If the player left clicks
        {
            animator.SetBool("Swing 1", true);                                                                              //Allows the animation to play
            GetComponent<BoxCollider>().enabled = true;                                                                     //Activates the shovel hitbox
            canSecondSwing = true;

            if (Input.GetKey(KeyCode.Mouse1) && canSecondSwing)                                                         //If the player wants to do a combo
            {
                animator.SetBool("Swing 2", true);                                                                          //Allows second animation to play
                GetComponent<BoxCollider>().enabled = true;                                                                 //Activates the shovel hit box
                canSecondSwing = false;
                Invoke(nameof(Cooldown), .75f);                                                                               //Calls on the other function
            }
            else
            {
                Invoke(nameof(Cooldown), .75f);                                                                               //Calls on the other function
            }
        }
    }

    void Cooldown()                                                                                                         //Function for stopping the swing
    {
        animator.SetBool("Swing 1", false);                                                                                 //Stops the swing animation
        animator.SetBool("Swing 2", false);                                                                                 //Stops the swing animation
        GetComponent<BoxCollider>().enabled = false;                                                                        //Disables the hitbox
    }
}