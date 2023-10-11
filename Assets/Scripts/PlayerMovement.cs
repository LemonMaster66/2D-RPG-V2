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
    public float Decceleration = 10f;
    public float JumpForce = 10;
    public int ClimbSpeed = 150;

    [Header("Player States")]
    public bool Grounded;
    public bool FacingRight;
    public bool AgainstWall;
    public bool Running;
    public bool Turning;
    public bool Climbing;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void StunnedTime()
    {
        StunnedTimer -= Time.deltaTime;
        if(StunnedTimer <= 0f)//Done
        {
            StunnedTimer = 0;
            if(FacingRight) transform.localScale = new Vector2(0.7589918f, transform.localScale.y);
            else transform.localScale = new Vector2(-0.7589918f, transform.localScale.y);
            return;
        }
        if(FacingRight) transform.localScale = new Vector2(0.5f, transform.localScale.y);
        else transform.localScale = new Vector2(-0.5f, transform.localScale.y);
        movementX = 0;
    }

    private void FlipPlayer()
    {
        if(FacingRight)
        {
            FacingRight = false;
            transform.localScale = new Vector2(transform.localScale.x*-1, transform.localScale.y);
        }
        else
        {
            FacingRight = true;
            transform.localScale = new Vector2(transform.localScale.x*-1, transform.localScale.y);
        }
    }

    void FixedUpdate()
    {
        //Run Every Frame that youre on the Ground
        if(Grounded)
        {
            ClimbSpeed = 150;
        }

        //Stunned = Cancel Movement
        if(StunnedTimer > 0)
        {
            StunnedTime();
            return;
        }
        
        //Flip Player Sprite
        if(transform.localScale.x > 0) FacingRight = true;
        else FacingRight = false;

        //Turning Logic
        if(Turning)
        {
            if(rb.velocity.x < 0.2 && rb.velocity.x > -0.2) //Start Moving Again
            {
                Turning = false;
                Running = true;
                rb.velocity = new Vector2(0, rb.velocity.y);
                if(FacingRight == true) movementX = -1;
                else movementX = 1;
                return;
            }
            else //Turning
            {
                movementX = 0;
                rb.velocity = new Vector2(rb.velocity.x/(Decceleration+1), rb.velocity.y);
                return;
            }
        }

        //Climbing Logic
        if(Climbing)
        {
            if(!AgainstWall)
            {
                Climbing = false;
                return;
            }
            if(rb.velocity.y > 0) ClimbSpeed -=3;

            if (rb.velocity.magnitude > ClimbSpeed/10)
            {
                Vector2 newVelocity = rb.velocity;
                newVelocity.x = 0f;
                newVelocity = Vector2.ClampMagnitude(newVelocity, ClimbSpeed/10);
                newVelocity.x = rb.velocity.x;
                rb.velocity = newVelocity;
            }
            
            if(ClimbSpeed > 0)
            {
                Vector2 ClimbMotion = new Vector2(0, 2*Acceleration*20);
                rb.AddForce(ClimbMotion);
            }
            else
            {
                Vector2 ClimbMotion = new Vector2(0, ClimbSpeed/5);
                rb.AddForce(ClimbMotion);
            }
        }


        //Keep Running While Not Pressing Anything
        if(movementX == 0 && Running && !AgainstWall && Grounded)
        {
            if(FacingRight == true) movementX = 1;
            else movementX = -1;
        }

        //Bonk / Climb
        if(Running && AgainstWall && !Climbing)
        {
            if(Grounded)
            {
                Debug.Log("Bonk");
                Running = false;
                playerSFX.Bonk.Play();
                StunnedTimer = 0.5f;
                return;
            }
            else //Running while Against a Wall and Not Grounded
            {
                Climbing = true;
            }
        }

        //Max Speed Cap
        if (rb.velocity.magnitude > Speed/10)
        {
            Vector2 newVelocity = rb.velocity;
            newVelocity.y = 0f;
            newVelocity = Vector2.ClampMagnitude(newVelocity, Speed/10);
            newVelocity.y = rb.velocity.y;
            rb.velocity = newVelocity;
        }

        //Player Has Turned Around Since Last Frame        
        if(movementX == LastmovementX *-1 && movementX !=0 && !Climbing) 
        {
            if(Running == false) //Walking
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                LastmovementX = movementX;
                return;
            }
            else //Running
            {
                if(Grounded)
                {
                    if(rb.velocity.x < 1 && rb.velocity.x > -1)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        LastmovementX = movementX;
                        return;
                    }
                    else if(rb.velocity.x > 1 || rb.velocity.x < -1)
                    {
                        Turning = true;
                        return;
                    }       
                }
            }
        }

        //Player has Stopped Moving Since
        else if(movementX == 0 && LastmovementX != 0 && !Climbing)
        {
            if(Running == false) //Walking
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                LastmovementX = movementX;
                return;
            }
            else //Running
            {
                Debug.Log("Stopped While Running");
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
        }

        //Running Logic Only When Grounded
        if(Running && Grounded)
        {
            Speed = Speed2;
        }
        else if(!Running && Grounded)
        {
            Speed = Speed1;
        }

        //
        if(Running && !Grounded && movementX != 0)
        {

        }
        else
        {
            movement = Vector2.right * movementX;

            if(movementX > 0)
            {
                if(!FacingRight)
                {
                    transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
                }
            }
            else if(movementX < 0)
            {
                if(FacingRight)
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
        if(StunnedTimer > 0 || Turning)
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

        else if(Climbing && AgainstWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            if(ClimbSpeed < 10) JumpForces = Vector2.up * 6;
            else JumpForces = Vector2.up * (ClimbSpeed+10)/10;

            if(FacingRight)
            {
                JumpForces += Vector2.left * JumpForce;
                if(movementX != 1) FlipPlayer();
            }
            else
            {
                JumpForces += Vector2.right * JumpForce;
                if(movementX != -1) FlipPlayer();
            }
        }

        rb.AddForce(JumpForces*60);
        ClimbSpeed -= 50;
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
