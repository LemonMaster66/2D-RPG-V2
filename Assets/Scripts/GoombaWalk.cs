using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaWalk : MonoBehaviour
{
    public float Speed = 2.0f;
    public float MovementX = 1.0f;
    public bool isDead = false;
    private Rigidbody2D rb;
    public LayerMask CollideWith;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            rb.velocity = new Vector2(MovementX * Speed, rb.velocity.y);
        }
    }

    public void Die()
    {
        isDead = true;
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector2 pushDirection = transform.position - playerObject.transform.position;
        rb.AddForce(pushDirection * 7, ForceMode2D.Impulse);
        gameObject.layer = 10;
        gameObject.tag = "DeadEnemy";
        transform.GetComponentInChildren<CapsuleCollider2D>().enabled = false;
    }

    public void Flip()
    {
        MovementX *= -1;
        transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
    }
}