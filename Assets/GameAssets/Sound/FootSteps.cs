using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootSteps : MonoBehaviour
{
    private PlayerMain mainScript;
    private void Start()
    {
        mainScript = GetComponentInParent<PlayerMain>();
    }
    public void FootStepSound()
    {
        mainScript.FootSteps();
    }
}
