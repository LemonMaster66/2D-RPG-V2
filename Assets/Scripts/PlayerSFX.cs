using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Player")]
    public AudioSource Walk;
    public AudioSource Run;
    public AudioSource Jump;
    public AudioSource Land;

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


    private bool HasJumped;
    private bool HasTurned;

    //private int RandomClip = 0;

    void FixedUpdate()
    {
        //Moving Grounded Not Stunned and Not Climbing
        if(playerMovement.MovementX !=0 && playerMovement.Grounded && playerMovement.StunnedTimer == 0 && playerMovement.Climbing == false)
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
        }

        if(playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y > 0 && playerMovement.Running) //Climbing Up a Wall
        {
            // RandomClip = Random.Range(0,4);
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

        if(playerMovement.Grounded && HasJumped)
        {
            HasJumped = false;
            Land.Play();
        }

        else if(playerMovement.Jumping && !HasJumped) //Player is jumping and you havent triggered a jump yet
        {
            HasJumped = true;
            Jump.Play();
        }
    }
}
