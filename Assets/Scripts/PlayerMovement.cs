using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Main Properties")]
    public int Speed = 100;
    public float Acceleration = 10f;
    public float JumpForce = 10;

    [Header("Player States")]
    public bool Grounded;
    public bool FacingRight;
    public bool AgainstWall;
    public bool Running;
    public bool Turning;
    public bool Stunned;
    public float StunnedTimer;

    private int Speed1 = 80;
    private int Speed2 = 150;
    //private int Speed3 = 150;

    [Header("Debug Stats")]
    public Vector2 RigidbodyVelocityStat;
    private Rigidbody2D rb;
    private Vector2 movement;
    public float movementX;
    private float LastmovementX;
    //private float movementY;

    [Header("Scripts")]
    public PlayerSFX playerSFX;


    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void StunnedTime()
    {
        StunnedTimer -= Time.deltaTime;
        if(StunnedTimer < 0f)
        {
            Stunned = false;
        }
    }

    void FixedUpdate()
    {
        if(Stunned)
        {
            StunnedTime();
            return;
        }
        
        if(transform.localScale.x > 0) FacingRight = true;
        else FacingRight = false;

        //Keep Running While Not Pressing Anything
        if(movementX == 0 && rb.velocity.x >= 0.2 && Running)
        {
            movementX = LastmovementX;
        }
        else if(movementX == 0 && rb.velocity.x <= -0.2 && Running)
        {
            movementX = LastmovementX;
        }

        if(Running && !AgainstWall)
        {
            if(FacingRight == true) movementX = 1;
            else movementX = -1;
        }
        else if(Running && AgainstWall) //Bonk
        {
            if(rb.velocity.x > 5 || rb.velocity.x < -5)
            {
                Debug.Log("Bonk");
                Running = false;
                playerSFX.Bonk.Play();
                StunnedTimer = 0.5f;
                Stunned = true;
                return;
            }
        }
        
        if (rb.velocity.magnitude > Speed/10) //Max Speed Cap
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = 0f;
            newVelocity = Vector3.ClampMagnitude(newVelocity, Speed/10);
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }
        
        if(movementX == LastmovementX *-1 && movementX !=0) //Player Has Turned Around Since Last Frame
        {
            if(Running == false) //[Walking] instantly change direction
            {
                Debug.Log("You Just Turned Around Walking");
                rb.velocity = new Vector2(0, rb.velocity.y);
                LastmovementX = movementX;
                return;
            }
            else //[Running] Slow Down To A Stop Before Moving
            {
                if(Grounded && rb.velocity.x < 1)//slower than 1
                {
                    Debug.Log("You Just Turned Around Run Walking");
                    rb.velocity = new Vector2(0, rb.velocity.y);
                    LastmovementX = movementX;
                    return;
                }
                else if(Grounded && rb.velocity.x > 1)//faster than 1
                {
                    Debug.Log("You Just Turned Around Running");
                    if(rb.velocity.x < -1 || rb.velocity.x > 1) Turning = true;
                }
            }
        }
        else if(movementX == 0 && LastmovementX != 0) //Player has Stopped Moving Since
        {
            if(Running == false) //[Walking] instantly change direction
            {
                Debug.Log("You Just Stopped Walking Buster");
                rb.velocity = new Vector2(0, rb.velocity.y);
                LastmovementX = movementX;
                return;
            }
            else //[Running] Slow Down To A Stop Before Moving
            {
                Debug.Log("You Just Stopped Running Buster");
                rb.velocity = new Vector2(0, rb.velocity.y);
                LastmovementX = movementX;
                return;
            }
        }

        if(Turning)
        {
            if(rb.velocity.x < 0.2 && rb.velocity.x > -0.2)
            {
                Turning = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                movementX = movementX*-1;
                return;
            }
            else
            {
                Debug.Log("Turning Around");
            }
        }

        //Running in Air
        if(Running && Grounded)
        {
            Speed = Speed2;
        }
        else if(!Running && Grounded)
        {
            Speed = Speed1;
        }

        if(Running && !Grounded && movementX != 0)
        {

        }
        else
        {
            movement = Vector2.right * movementX;

            if(movementX > 0)//moving right
            {
                if(transform.localScale.x < 0) //Looking Left
                {
                    transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
                }
            }
            else if(movementX < 0) //moving left
            {
                if(transform.localScale.x > 0) //Looking Right
                {
                    transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        rb.AddForce((movement*Speed*Acceleration)/10);
        LastmovementX = movementX;

        //DebugStats
        RigidbodyVelocityStat = new Vector2(rb.velocity.x, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext movementValue)
    {
        Vector2 inputVector = movementValue.ReadValue<Vector2>();
        movementX = inputVector.x;
    }

    public void OnRun(InputAction.CallbackContext runValue)
    {
        float Runningfloat = runValue.ReadValue<float>();
        if(Runningfloat == 1) //If you are running
        {
            Running = true;
        }
        else
        {
            Running = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(Stunned)
        {
            return;
        }

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
    public void SetAgainstWall(bool state)
    {
        AgainstWall = state;
    }
}
