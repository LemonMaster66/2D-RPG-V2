using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public int CrouchSpeed = 30;

    [Header("Player States")]
    public bool Grounded;
    public bool Jumping;
    public bool HoldingJump;
    public bool FacingRight;
    public bool AgainstWall;
    public bool Running;
    public bool Crouching;
    public bool Diving;
    public bool GroundPounding;
    public bool AirStalling;
    public bool Turning;
    public bool Climbing;

    [Header("Timer / Countdowns")]
    public float StunnedTimer;
    public float WallJumpTimer;
    public float HurtTimer;
    public float GroundPoundTimer;
    

    private bool RunningStash;

    private int Speed1 = 80;
    private int Speed2 = 150;
    

    [Header("Debug Stats")]
    public Vector2 RigidbodyVelocityStat;
    public Rigidbody2D rb;
    public Animator animator;
    private Vector2 movement;
    public float MovementX;
    public float MovementXStash = 1;
    public float GeneralCounter = 1;

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
            if(FacingRight) transform.localScale = new Vector2(3f, transform.localScale.y);
            else transform.localScale = new Vector2(-3f, transform.localScale.y);
            return;
        }
        if(FacingRight) transform.localScale = new Vector2(2.5f, transform.localScale.y);
        else transform.localScale = new Vector2(-2.5f, transform.localScale.y);
        animator.Play("Slide Down");
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

    private void Hurt()
    {
        HurtTimer -= Time.deltaTime;
        if(HurtTimer <= 0f)//Done
        {
            HurtTimer = 0;
        }
        return;
    }

    private void GroundPoundingLogic()
    {
        rb.velocity = new Vector2(0, GeneralCounter-=2);
        GroundPounding = true;
        animator.Play("GroundPound Fall");
        return;
    }

    private void GroundPoundTime()
    {
        GroundPoundTimer -= Time.deltaTime;
        if(GroundPoundTimer <= 0f)//Done
        {
            GroundPoundTimer = 0;
            rb.isKinematic = false;
        }
        rb.velocity = new Vector2(0,0);
        animator.Play("GroundPound");
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

        //Timers
        if(StunnedTimer > 0)
        {
            StunnedTime();
            return;
        }
        if(WallJumpTimer > 0)
        {
            WallJumpInputCoolDown();
        }
        if(HurtTimer > 0)
        {
            Hurt();
            return;
        }
        if(GroundPoundTimer > 0)
        {
            GroundPoundTime();
            return;
        }


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

            if(MovementX == 0 && StunnedTimer == 0 && animator.GetBool("SpearNonsense") == false)                  animator.Play("Idle");
            else if(MovementX != 0 && !Running && StunnedTimer == 0 && animator.GetBool("SpearNonsense") == false) animator.Play("Walk");
            else if(MovementX != 0 && Running && StunnedTimer == 0 && animator.GetBool("SpearNonsense") == false)  animator.Play("Run");
        }
        else
        {
            if(rb.velocity.y > 0 && !GroundPounding && !Diving && GroundPoundTimer == 0 && !AgainstWall && animator.GetBool("SpearNonsense") == false)      animator.Play("Jump");
            else if(rb.velocity.y < 0 && !GroundPounding && !Diving && GroundPoundTimer == 0 && !AgainstWall && animator.GetBool("SpearNonsense") == false) animator.Play("Fall");
        }

        //Run Every Frame that youre Crouching
        if(Jumping == true)
        {
            if(Grounded) Jumping = false;
            else if(AgainstWall && Climbing) Jumping = false;
        }

        //Run Every Frame that youre Crouching
        if(Crouching && !GroundPounding)
        {
            if(Grounded) //CrouchWalk
            {
                if(!Running) Speed = CrouchSpeed;
                transform.localScale = new Vector3(transform.localScale.x, 2.5f, transform.localScale.z);
            }
            else if(!Grounded && MovementX != 0 && !AgainstWall && Running) //Dive
            {
                Diving = true;
            }
            else if(!Grounded && MovementX == 0 && !AgainstWall && Jumping) //Ground Pound
            {
                rb.velocity = new Vector2(0,0);
                GroundPounding = true;
                GeneralCounter = 1;
            }
        }

        //Run Every Frame that youre Diving
        if(Diving)
        {
            if(!AgainstWall && !Grounded)
            {
                if(FacingRight) rb.velocity = new Vector2(20, -20);
                else rb.velocity = new Vector2(-20, -20);
                animator.Play("Dive");
                return;
            }
            else
            {
                Diving = false;
            }
        }

        //Run Every Frame that youre GroundPounding
        if(GroundPounding)
        {
            if(!Grounded)
            {
                GroundPoundingLogic();
                Crouching = true;
            }
            else
            {
                GroundPounding = false;
                Crouching = false;
            }
        }
        
        //Auto Flip Player Sprite
        if(transform.localScale.x > 0) FacingRight = true;
        else FacingRight = false;

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
                else //Not Against a Wall and not Jumping = Mantle
                {
                    if(Running)
                    {
                        MovementX = MovementXStash;
                        Climbing = false;
                        rb.velocity = new Vector2(rb.velocity.x, 0);
                        if(FacingRight) transform.position = new Vector3(transform.position.x + 0.3f, transform.position.y + 0.2f, transform.position.z);
                        else            transform.position = new Vector3(transform.position.x - 0.3f, transform.position.y + 0.2f, transform.position.z);
                        rb.velocity += new Vector2(6*MovementX, 0);
                    }
                }
            }
            else //Climbing and Against Wall
            {
                if(rb.velocity.y > 0 && Running && animator.GetBool("SpearNonsense") == false) animator.Play("Climb Up");
                else if(rb.velocity.y < 0 && animator.GetBool("SpearNonsense") == false)       animator.Play("Slide Down");

                ClimbSpeed -=3;
            
                //Vector2 ClimbMotion = new Vector2(0, 2*Acceleration * (ClimbSpeed/15));
                if(ClimbSpeed > 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y+(ClimbSpeed/30));

                if (AgainstWall && rb.velocity.y > 15) //speed cap
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
                    playerSFX.Wall.clip = playerSFX.Bonk;
                    playerSFX.Wall.volume = 1;
                    playerSFX.Wall.Play();
                    StunnedTimer = 0.5f;
                    return;
                }
            }
            else Climbing = true;
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
        if(MovementX == MovementXStash *-1 && MovementX !=0 && !Climbing) 
        {
            if(!Running) //Walking
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
                MovementXStash = MovementX;
            }
            else //Running
            {
                if(Grounded)
                {
                    if(rb.velocity.x < 1 && rb.velocity.x > -1)
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                        MovementXStash = MovementX;
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
        else if(MovementX == 0 && MovementXStash != 0)
        {
            if(Running == false) //Walking
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else //Running
            {
                Debug.Log("Stopped While Running");
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
        }

        //Turning Logic
        if(Turning)
        {
            if(rb.velocity.x < 0.01 && rb.velocity.x > -0.01) //Start Moving Again
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

        //Running Logic
        if(Running && Grounded)
        {
            Speed = Speed2;
        }
        else if(!Running && Grounded)
        {
            Speed = Speed1;
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
        RunningStash = Running;
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
    public void OnCrouch(InputAction.CallbackContext crouchValue)
    {
        if(GroundPounding) return;

        float Crouchingfloat = crouchValue.ReadValue<float>();
        if(Crouchingfloat == 1) //If you are Crouching
        {
            Crouching = true;
            if(Grounded)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.05f, transform.position.z);
            }
        }
        else
        {
            if(Crouching)
            {
                StopCrouching();
            }
            Crouching = false;
        }
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        if(StunnedTimer > 0 || Turning || GroundPounding || GroundPoundTimer > 0) return;
        if(Crouching)
        {
            StopCrouching();
            Crouching = false;
        }

        float Jumpingfloat = context.ReadValue<float>();
        if(Jumpingfloat == 1) HoldingJump = true;
        else HoldingJump = false;

        if(context.started) Jump();
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
    
    
    public void StopCrouching()
    {
        transform.localScale = new Vector3(transform.localScale.x, 3f, transform.localScale.z);
        if(Grounded) transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        
        if(!Running) Speed = Speed1;
        else Speed = Speed2;

        GeneralCounter = 1;
    }

    public void Stomp()
    {
        if(!HoldingJump) rb.velocity = new Vector2(rb.velocity.x, 6);
        else rb.velocity = new Vector2(rb.velocity.x, 15);
    }
}
