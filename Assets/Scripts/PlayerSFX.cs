using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Sounds")]
    public AudioSource Walk;
    public AudioSource Run;
    public AudioSource Jump;
    public AudioSource Land;
    public AudioSource Bonk;
    public AudioSource WallClimb;
    public AudioSource WallSlide;

    private bool HasJumped;
    private bool HasTurned;

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
            else
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
            if(!WallClimb.isPlaying) WallClimb.Play();
            if(WallSlide.isPlaying) WallSlide.Stop();
            WallClimb.volume = playerMovement.rb.velocity.y/8;
        }
        else if(playerMovement.Climbing && playerMovement.AgainstWall && playerMovement.rb.velocity.y < 0) //Sliding Down a Wall
        {
            if(WallClimb.isPlaying) WallClimb.Stop();
            if(!WallSlide.isPlaying)WallSlide.Play();
            WallSlide.volume = (playerMovement.rb.velocity.y/8)*-1;
        }
        else
        {
            WallClimb.Stop();
            WallSlide.Stop();
        }

        if(playerMovement.Grounded && HasJumped)
        {
            HasJumped = false;
            Land.Play();
        }

        if(playerMovement.Jumping && !HasJumped) //Player is jumping and you havent triggered a jump yet
        {
            HasJumped = true;
            Jump.Play();
        }
    }
}
