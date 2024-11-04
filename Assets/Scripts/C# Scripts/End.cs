using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] PlayerMovement movement;
    [SerializeField] Swing swing;
    [SerializeField] PlayerCam cam;
    [SerializeField] GameObject UI1;
    [SerializeField] GameObject UI2;
    [SerializeField] GameObject UI3;

    void OnTriggerEnter(Collider other)
    {
        movement.enabled = false;
        swing.enabled = false;
        cam.enabled = false;
        UI1.SetActive(true);
        UI2.SetActive(true);
        UI3.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}