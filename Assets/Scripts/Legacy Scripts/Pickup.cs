using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] bool inRange; // Checks if the player is close to the object, can be seen in Unity

    void Update() // Runs every frame
    {
        if (inRange == true && Input.GetKeyDown("e")) // Checks that they player is in range and presses the input
        {
            Debug.Log("Picked up"); // Confirms in Unity that the script ran correctly
            Destroy(gameObject); // Removes the object from the game world so it doesn't duplicate
        }
    }

    void OnTriggerEnter(Collider other) // The check when the player is close
    {
        if (other.CompareTag("Player")) //Checks if it is the player
        {
            inRange = true; // Makes its so the player can pick up the object
        }
    }

    void OnTriggerExit(Collider other) // The check when the player is too far
    {
        if (other.CompareTag("Player")) // Checks if it is the player
        {
            inRange = false; // Makes its so the player can't pick up the object
        }
    }
}