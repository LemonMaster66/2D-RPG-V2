using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public bool AgainstWall; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject || other.tag == "Spear") return;

        playerMovement.SetAgainstWall(true);
        AgainstWall = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject || other.tag == "Spear") return;

        playerMovement.SetAgainstWall(false);
        AgainstWall = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == playerMovement.gameObject || other.tag == "Spear") return;

        playerMovement.SetAgainstWall(true);
        AgainstWall = true;
    }
}
