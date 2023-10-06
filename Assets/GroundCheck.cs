using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundCheck : MonoBehaviour
{

    public PlayerMovement playerMovement;
    public bool Grounded; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;

        playerMovement.SetGrounded(true);
        Grounded = true;
        Debug.Log("Grounded");
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;

        playerMovement.SetGrounded(false);
        Grounded = false;
        Debug.Log("Not Grounded");
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject)
            return;

        playerMovement.SetGrounded(true);
        Grounded = true;
    }
}