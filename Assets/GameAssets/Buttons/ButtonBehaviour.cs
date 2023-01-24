using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour
{
    private Animator animator;

    private int selectedID;
    private int mouseOverID;
    private int pressedID;
    private int normalID;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        selectedID = Animator.StringToHash("Selected");
        mouseOverID = Animator.StringToHash("MouseOver");
        pressedID = Animator.StringToHash("Pressed");
        normalID = Animator.StringToHash("Normal");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
