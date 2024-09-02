using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{

    [SerializeField] private Animator animator;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            animator.SetBool("Swing", true);

            Invoke(nameof(Cooldown), 1f);
        }
    }

    void Cooldown()
    {
        animator.SetBool("Swing", false);
    }
}
