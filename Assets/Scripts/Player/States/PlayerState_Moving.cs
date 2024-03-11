using FMOD.Studio;
using UnityEngine;

public class PlayerState_Moving : PlayerState
{
    
    protected Vector3 currMoveVelocity;
    protected Vector3 moveDampVelocity;
    protected float moveSpeed;

    protected float currentGravitationalForce = 0f;
    public override void Enter()
    {
        base.Enter();
        currMoveVelocity = playerController.currMoveVelocity;
        moveDampVelocity = playerController.moveDampVelocity;
    }
    public override void UpdateState()
    {
        Move(inputActions.Movement.Move.ReadValue<Vector2>());
        UpdateSound();
    }
    public override void Exit()
    {
        base.Exit();
        playerController.currMoveVelocity = currMoveVelocity;
        playerController.moveDampVelocity = currMoveVelocity;
    }
    public void Move(Vector2 value)
    {
        currentGravitationalForce += (currentGravitationalForce - playerController.GravityStrength) * Time.deltaTime;
        if (playerController.CurrentState != playerController.JumpState && playerController.GetComponent<CharacterController>().isGrounded)
        {
            currentGravitationalForce = -playerController.GravityStrength;
        }
        //Debug.Log(currentGravitationalForce);
        Mathf.Clamp(currentGravitationalForce, -2500f, playerController.JumpStrength);

        Vector3 playerMovement = new Vector3(value.x, currentGravitationalForce, value.y);
        //playerMovement.Normalize();
        Vector3 moveVector = playerController.transform.TransformDirection(playerMovement);
        currMoveVelocity = Vector3.SmoothDamp(currMoveVelocity, (moveVector * moveSpeed), ref moveDampVelocity, playerController.MovementSmoothing);
        //currMoveVelocity.y += Mathf.Clamp(currentGravitationalForce, -1000f, playerController.JumpStrength);
        playerController.GetComponent<CharacterController>().Move(currMoveVelocity* Time.deltaTime);
    }

    public override void OnSneak()
    {
        playerController.TransitionToState(playerController.SneakState);
    }

    public override void OnSprint()
    {
        playerController.TransitionToState(playerController.SprintState);
    }

    public override void OnJump()
    {
        if(playerController.GetComponent<CharacterController>().isGrounded) 
        {
            //playerController.TransitionToState(playerController.JumpState);
        }
    }

    private void UpdateSound()
    {
        if (inputActions.Movement.Move.ReadValue<Vector2>().magnitude > 0f)
        {
            PLAYBACK_STATE playbackstate;
            AudioController.Instance.playerFootsteps.getPlaybackState(out playbackstate);
            if (playbackstate.Equals(PLAYBACK_STATE.STOPPED))
            {
                AudioController.Instance.playerFootsteps.start();
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(AudioController.Instance.playerFootsteps, playerController.transform);
            }
        }
        else
        {
            FMODUnity.RuntimeManager.DetachInstanceFromGameObject(AudioController.Instance.playerFootsteps);
            AudioController.Instance.playerFootsteps.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
