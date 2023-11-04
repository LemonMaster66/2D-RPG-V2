using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearCollision : MonoBehaviour
{
    public Spear spear;
    public bool collided = false;
    public GameObject CollidedObject;

    void Awake()
    {
        spear = FindObjectOfType<Spear>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(collided || spear.isRecalling) return;
        CollidedObject = other.gameObject;

        if (other.tag == "Enemy")
        {
            spear.CollideEnemy();
            other.GetComponent<GoombaWalk>().Die();
            other.GetComponent<Animator>().Play("Death");
        }
        else spear.CollideGround();
        
        PlatformEffector2D platformEffector2D = transform.parent.GetComponent<PlatformEffector2D>();
        if (transform.parent.rotation.eulerAngles.z > 90 && transform.parent.rotation.eulerAngles.z < 270) platformEffector2D.rotationalOffset = 180;
        else if (transform.parent.rotation.eulerAngles.z < 90) platformEffector2D.rotationalOffset = 0;
    }
}
