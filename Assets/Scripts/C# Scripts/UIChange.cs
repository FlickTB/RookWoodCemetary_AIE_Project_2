using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIChange : MonoBehaviour
{
    [Header("UI Text")]                                                                                                     //Header for Unity
    [SerializeField] private string popUpText;                                                                              //Text that changes

    [Header("Text Reference")]                                                                                              //Header for Unity
    [SerializeField] TextMeshProUGUI info;                                                                                  //Reference UI

    void OnTriggerEnter(Collider other)                                                                                     //When the player enters the trigger
    {
        info.text = popUpText;                                                                                              //Display the text
    }

    void OnTriggerExit(Collider other)                                                                                      //When the player leaves the trigger
    {
        info.text = "";                                                                                                     //Removes the text
    }
}