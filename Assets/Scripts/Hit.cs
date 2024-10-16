using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    public GameObject deathVFX;                                                                                             //Death effect reference
    void OnTriggerEnter(Collider other)                                                                                     //Function called on entering a trigger
    {
        if(other.CompareTag("Weapon"))                                                                                      //If the object has the weapon tag
        {

            Instantiate(deathVFX, transform.position, transform.rotation);                                                  //Play the death effect

            Destroy(gameObject);                                                                                            //Destroy the object
        }
    }
}