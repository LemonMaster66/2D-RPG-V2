using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundCheck : MonoBehaviour
{

    public PlayerMovement playerMovement;
    public Spear spear;
    public bool Grounded; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject) return;

        else if (other.tag == "Spear" && playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y > 0.1)
        {
            playerMovement.ClimbSpeed += 100;
            spear.Recall();
            return;
        }

        playerMovement.SetGrounded(true);
        Grounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject) return;

        playerMovement.SetGrounded(false);
        Grounded = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject) return;

        else if (other.tag == "Spear" && playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y > 0.1) return;

        playerMovement.SetGrounded(true);
        Grounded = true;
    }
}