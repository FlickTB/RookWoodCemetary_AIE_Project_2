using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{

    public GameObject deathVFX;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon"))
        {

            Instantiate(deathVFX, transform.position, transform.rotation);

            Destroy(gameObject);
        }
    }
}