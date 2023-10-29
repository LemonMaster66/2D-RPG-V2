using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public bool Jumping;
    public bool HoldingJump;
    public bool FacingRight;
    public bool AgainstWall;
    public bool Running;
    public bool Turning;
    public bool Climbing;
    public float StunnedTimer;
    public float WallJumpTimer;

    private int Speed1 = 80;
    private int Speed2 = 150;
    //private int Speed3 = 150;

    [Header("Debug Stats")]
    public Vector2 RigidbodyVelocityStat;
    public Rigidbody2D rb;
    private Vector2 movement;
    public float MovementX;
    public float MovementXStash = 1;
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
    }

    private void WallJumpInputCoolDown()
    {
        WallJumpTimer -= Time.deltaTime;
        if(WallJumpTimer <= 0f)//Done
        {
            WallJumpTimer = 0;
        }
        return;
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




    //********************************************************************************
    //********************************************************************************
    //********************************************************************************
    //********************************************************************************




    void FixedUpdate()
    {
        //DebugStats
        RigidbodyVelocityStat = new Vector2(rb.velocity.x, rb.velocity.y);

        //Run Every Frame that youre on the Ground
        if(Grounded)
        {
            Climbing = false;

            //Run Every Frame that youre on the Ground and Running
            if(Running)
            {
                ClimbSpeed = 150;
            }
            else
            {
                ClimbSpeed = 0;
            }
        }

        if(Jumping == true)
        {
            if(Grounded) Jumping = false;
            else if(AgainstWall && Climbing) Jumping = false;
        }

        //Stunned = Cancel Movement
        if(StunnedTimer > 0)
        {
            StunnedTime();
            return;
        }
        if(WallJumpTimer > 0)
        {
            WallJumpInputCoolDown();
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
                if(FacingRight == true) MovementX = -1;
                else MovementX = 1;
                return;
            }
            else //Turning
            {
                MovementX = 0;
                rb.velocity = new Vector2(rb.velocity.x/(Decceleration+1), rb.velocity.y);
                return;
            }
        }

        //Climbing Logic
        if(Climbing)
        {
            if(Grounded)
            {
                MovementX = MovementXStash;
                Climbing = false;
            }
            if(!AgainstWall)
            {
                if(Jumping) //Not against a Wall and Jumping = Wall Jumping
                {
                    if(Running)
                    {
                        return;
                    }
                    else
                    {
                        if(WallJumpTimer > 0) return;
                    }
                }
                else //Not against a Wall not Jumping = Mantle
                {
                    if(Running)
                    {
                        MovementX = MovementXStash;
                        Climbing = false;
                    }
                }
                
                
            }
            else
            {
                ClimbSpeed -=3;
            
                //Vector2 ClimbMotion = new Vector2(0, 2*Acceleration * (ClimbSpeed/15));
                if(ClimbSpeed > 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y+(ClimbSpeed/30));

                if (AgainstWall && rb.velocity.y > 15)
                {
                    Vector2 newVelocity = rb.velocity;
                    newVelocity.x = 0f;
                    newVelocity = Vector2.ClampMagnitude(newVelocity, 15);
                    newVelocity.x = rb.velocity.x;
                    rb.velocity = newVelocity;
                }
            }
        }


        if(MovementX == 0 && Running) //Not Pressing Anything
        {
            if(FacingRight == true) MovementX = 1;
            else MovementX = -1;
        }

        //Bonk / Climb
        if(AgainstWall && !Climbing)
        {
            if(Grounded)
            {
                if(Running)
                {
                    Debug.Log("Bonk");
                    playerSFX.Wall.clip = playerSFX.Bonk;
                    playerSFX.Wall.volume = 1;
                    playerSFX.Wall.Play();
                    StunnedTimer = 0.5f;
                    return;
                }
            }
            else
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

        // //Player Has Turned Around Since Last Frame        
        // if(MovementX == MovementXStash *-1 && MovementX !=0 && !Climbing) 
        // {
        //     if(Running == false) //Walking
        //     {
        //         rb.velocity = new Vector2(0, rb.velocity.y);
        //         MovementXStash = MovementX;
        //     }
        //     else //Running
        //     {
        //         if(Grounded || Climbing)
        //         {
        //             if(rb.velocity.x < 1 && rb.velocity.x > -1)
        //             {
        //                 rb.velocity = new Vector2(0, rb.velocity.y);
        //                 MovementXStash = MovementX;
        //                 return;
        //             }
        //             else if(rb.velocity.x > 1 || rb.velocity.x < -1)
        //             {
        //                 Turning = true;
        //                 return;
        //             }       
        //         }
        //     }
        // }

        // //Player has Stopped Moving Since
        // else if(MovementX == 0 && MovementXStash != 0)
        // {
        //     if(Running == false) //Walking
        //     {
        //         rb.velocity = new Vector2(0, rb.velocity.y);
        //     }
        //     else //Running
        //     {
        //         Debug.Log("Stopped While Running");
        //         rb.velocity = new Vector2(0, rb.velocity.y);
        //         return;
        //     }
        // }

        //Running Logic Only When Grounded
        if(Running && Grounded)
        {
            Speed = Speed2;
        }
        else if(!Running && Grounded)
        {
            Speed = Speed1;
        }


        if(MovementX != MovementXStash && !Climbing)
        {
            if(Climbing) MovementX = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        if(Climbing && AgainstWall) return;
        else movement = Vector2.right * MovementX;

        //Sprite Flip Logic
        if(MovementX > 0)
        {
            if(!FacingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
            }
        }
        else if(MovementX < 0)
        {
            if(FacingRight)
            {
                transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);
            }
        }

        rb.AddForce((movement*Speed*Acceleration)/10);
    }




    //********************************************************************************
    //********************************************************************************
    //********************************************************************************
    //********************************************************************************





    public void OnMove(InputAction.CallbackContext movementValue)
    {
        MovementXStash = MovementX;
        Vector2 inputVector = movementValue.ReadValue<Vector2>();
        MovementX = inputVector.x;
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

        float Jumpingfloat = context.ReadValue<float>();
        if(Jumpingfloat == 1) HoldingJump = true;
        else HoldingJump = false;

        if(context.started)
        {
            Jump();
        }
    }
    void Jump()
    {
        Vector2 JumpForces = Vector2.zero;
        Jumping = true;

        //Normal Jump Logic
        if (Grounded)
        {
            JumpForces = Vector2.up * JumpForce;
            Grounded = false;
        }

        //Wall Jump Logic
        else if(Climbing && AgainstWall)
        {
            if(ClimbSpeed < 10) JumpForces = Vector2.up * 10; //Below 10 ClimbSpeed
            else if(ClimbSpeed >= 10 && ClimbSpeed < 80) JumpForces = Vector2.up * (ClimbSpeed+20)/20; //Above 10 but Below 80
            else JumpForces = Vector2.up*3; //Above 80

            if(!Running) WallJumpTimer = 0.1f;

            FlipPlayer();

            if(FacingRight) JumpForces += Vector2.right * JumpForce;
            else JumpForces += Vector2.left * JumpForce;

            AgainstWall = false;
            Jumping = true;
        }

        rb.AddForce(JumpForces*60);
        ClimbSpeed -= 60;
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
