using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] float rotateSpeed; //Sets the base speed of the rotation, changed in Unity
    
    void Update() // Update is called once per frame
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, 0, 0); // Rotates the light
    }
}