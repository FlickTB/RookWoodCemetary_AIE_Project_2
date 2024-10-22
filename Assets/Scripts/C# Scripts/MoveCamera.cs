using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;                                                                                        //A refernce for the camera's position ingame

    void Update()                                                                                                           //Called upon every frame
    {
        transform.position = cameraPosition.position;                                                                       //Tells the camera where it's supposed to move to
    }
}