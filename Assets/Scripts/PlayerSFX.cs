using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Animator animator;

    [Header("Player")]
    public AudioSource Walk;
    public AudioSource Run;
    public AudioSource Jump;
    public AudioSource Dive;

    [Header("Land Sfx")]
    public AudioSource Land;
    public AudioClip LandGround;
    public AudioClip GroundPound;
    public AudioClip Stomp;

    [Header("Wall Stuff")]
    public AudioSource Wall;
    public AudioClip WallClimb;
    public AudioClip WallSlide;
    public AudioClip WallJump;
    public AudioClip Bonk;
    
    [Header("Spear")]
    public AudioSource Spear;
    public AudioClip[] ThrowSpear;
    public AudioClip[] RecallSpear;

    [Header("SpearImpale")]
    public AudioSource Impale;
    public AudioClip[] ImpaleGround;
    public AudioClip[] ImpaleEnemy;
    public AudioClip[] CatchSpear;

    public bool HasJumped;
    public bool HasTurned;
    public bool HasDived;

    void FixedUpdate()
    {
        //Moving Grounded Not Stunned and Not Climbing
        if(playerMovement.MovementX !=0 && playerMovement.Grounded && playerMovement.StunnedTimer == 0 && playerMovement.Climbing == false && playerMovement.GroundPoundTimer <= 0)
        {
            if(!playerMovement.Running)
            {
                if(!Walk.isPlaying)
                {
                    Walk.pitch =1 + Random.Range(-0.2f, 0.2f);
                    Walk.Play();
                }
                if(Run.isPlaying) Run.Stop();
            }
            else if (!playerMovement.Crouching)
            {
                if(Walk.isPlaying) Walk.Stop();
                if(!Run.isPlaying) Run.Play();
            }
        }
        else //Not Moving or Stunned or Climbing
        {
            if(Walk.isPlaying) Walk.Stop();
            if(Run.isPlaying) Run.Stop();
            
            //animator.Play("Idle");
        }

        if(playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y > 0 && playerMovement.Running) //Climbing Up a Wall
        {
            Wall.clip = WallClimb;
            if(!Wall.isPlaying) Wall.Play();
            Wall.volume = playerMovement.rb.velocity.y/8;
        }
        else if(playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y < 0) //Sliding Down a Wall
        {
            Wall.clip = WallSlide;
            if(!Wall.isPlaying) Wall.Play();
            Wall.volume = (playerMovement.rb.velocity.y/8)*-1;
        }
        else if(Wall.clip != Bonk)
        {
            Wall.Stop();
        }

        if(playerMovement.Jumping && !HasJumped && playerMovement.HoldingJump) //Jump
        {
            HasJumped = true;
            Jump.Play();
        }
        if(playerMovement.Grounded || playerMovement.AgainstWall) //Reset Jump
        {
            HasJumped = false;
            playerMovement.Jumping = false;
            Dive.Stop();
        }
    
        if(playerMovement.Diving && !playerMovement.Grounded)
        {
            if(!Dive.isPlaying && !HasDived)
            {
                Debug.Log("Dive");
                HasDived = true;
                Dive.Play();
            }
        }
    }
}
