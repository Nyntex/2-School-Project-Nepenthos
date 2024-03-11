using System.Collections;
using System;
using UnityEngine;

public class PlayerState_Jump : PlayerState_Moving
{
    private float shortTimer = 0.2f;
    public override void Enter()
    {
        base.Enter();
        //shortTimer = 0.2f;
        shortTimer = 0.0f;
        currentGravitationalForce = 0.0f;
        //currMoveVelocity.y = playerController.JumpStrength;
        currMoveVelocity.y = 0f;
        moveSpeed = playerController.currMoveSpeed;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (playerController.GetComponent<CharacterController>().isGrounded && shortTimer <= 0f)
        {
            if (inputActions.Movement.Sprint.IsPressed())
            {
                if (playerController.currentStamina > 0.0f)
                {
                    playerController.TransitionToState(playerController.SprintState);
                }
            }
            else 
            {
                playerController.TransitionToState(playerController.WalkState);
            }
        }
        shortTimer -= Time.deltaTime;
    }
    public override void OnJump()
    {
        // maybe doube jump?
    }
    public override void OnSneak()
    {
        //base.OnSneak();
        //check with gd if this should be possible
    }
}