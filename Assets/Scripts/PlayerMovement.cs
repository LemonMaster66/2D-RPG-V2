using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    
    public int speed = 100;
    public float JumpForce = 10;

    private Rigidbody2D rb;
    private float movementX;
    private float movementY;
    public bool Grounded;


    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 movement = (Vector2.right * movementX);
        transform.Translate((movement * speed)/600);
    }

    public void OnMove(InputAction.CallbackContext movementValue)
    {  
        Vector2 inputVector = movementValue.ReadValue<Vector2>();

        movementX = inputVector.x;
        movementY = inputVector.y;

    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Jump();
        }
    }
    void Jump()
    {
        Vector2 JumpForces = Vector2.zero;

        if (Grounded)
        {
            JumpForces = Vector2.up * JumpForce;
        }

        rb.AddForce(JumpForces*60);
    }
    public void SetGrounded(bool state)
    {
        Grounded = state;
    }
}
