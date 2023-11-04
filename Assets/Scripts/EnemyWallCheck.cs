using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWallCheck : MonoBehaviour
{
    public GoombaWalk goombaWalk;
    public PlayerSFX playerSFX;
    public bool AgainstWall;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == goombaWalk.gameObject) return;

        if (other.tag == "Spear")
        {
            goombaWalk.Die();
            goombaWalk.GetComponent<Animator>().Play("Death");
        }

        if(!goombaWalk.isDead)
        {
            goombaWalk.Flip();
            AgainstWall = true;
        }
    }
}
