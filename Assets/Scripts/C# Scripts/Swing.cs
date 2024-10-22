using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    [SerializeField] private Animator animator;                                                                             //Refernce for the animation

    void Update()                                                                                                           //Function called every frame
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))                                                                               //If the player left clicks
        {
            animator.SetBool("Swing", true);                                                                                //Allows the animation to play
            GetComponent<BoxCollider>().enabled = true;                                                                     //Activates the shovel hitbox

            Invoke(nameof(Cooldown), 1f);                                                                                   //Calls on the other function
        }
    }

    void Cooldown()                                                                                                         //Function for stopping the swing
    {
        animator.SetBool("Swing", false);                                                                                   //Stops the swing animation
        GetComponent<BoxCollider>().enabled = false;                                                                        //Disables the hitbox
    }
}