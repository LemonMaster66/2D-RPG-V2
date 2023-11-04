using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GroundCheck : MonoBehaviour
{

    public PlayerMovement playerMovement;
    public PlayerSFX playerSFX;
    public Animator animator;
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
        if(other.tag == "Enemy")
        {
            other.GetComponent<GoombaWalk>().Die();
            other.GetComponent<Animator>().Play("Death");
            playerMovement.Stomp();
            playerSFX.Land.clip = playerSFX.Stomp;
            playerSFX.Land.Play();
        }
        else
        {
            playerSFX.Land.clip = playerSFX.LandGround;
            if(playerMovement.Jumping) playerSFX.Land.Play();
            playerSFX.HasJumped = false;
            playerSFX.HasDived = false;
        }

        playerMovement.SetGrounded(true);
        Grounded = true;

        if(playerMovement.GroundPounding)
        {
            playerMovement.GroundPoundTimer = 1;
            playerMovement.GroundPounding = false;
            playerMovement.rb.isKinematic = false;
            playerMovement.Crouching = false;
            
            playerSFX.Land.clip = playerSFX.GroundPound;
            playerSFX.Land.Play();
        }

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