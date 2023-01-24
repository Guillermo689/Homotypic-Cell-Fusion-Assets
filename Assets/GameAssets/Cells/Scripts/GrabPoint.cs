using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabPoint : MonoBehaviour
{
    private PlayerMain mainScript;
    void Start()
    {
        mainScript = GetComponentInParent<PlayerMain>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cell"))
        {
            mainScript.cellTograb = collision.gameObject;
        }
        if (collision.CompareTag("GuardianCell"))
        {
           mainScript.cellTograb = collision.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mainScript.cellTograb = null;
    }
}
