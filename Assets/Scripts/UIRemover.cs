using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class UIRemover : MonoBehaviour
{
    [SerializeField] float uiTime;                                                                                          //Refernce for when the UI disappears after set time
    [SerializeField] TextMeshProUGUI tutorial;                                                                              //Refernce for what UI is changed

    void Start()                                                                                                            //Function called at the start
    {
        Invoke(nameof(UIChange), uiTime);                                                                                   //Calls the other function
    }

    void OnTriggerEnter()                                                                                                   //Function called when player walks into a trigger
    {
        UIChange();                                                                                                         //Calls the other function
    }

    void UIChange()                                                                                                         //Function for the UI disappearing
    {
        Object.Destroy(tutorial);                                                                                           //Removes UI
    }
}