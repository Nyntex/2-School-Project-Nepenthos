using FMOD.Studio;
using UnityEngine;

public class PlayerState_Sprint : PlayerState_Moving
{
    private float soundRange;

    public override void Enter()
    {
        base.Enter();
        moveSpeed = playerController.SprintSpeed;
        playerController.currMoveSpeed = moveSpeed;
        AudioController.Instance.SetSoundArea(SoundAreaType.PLAYER, (float)PlayerMovingState.SPRINT);
        float min, max;
        EventDescription description;
        AudioController.Instance.playerFootsteps.getDescription(out description);
        description.getMinMaxDistance(out min, out max);
        soundRange = max * playerController.SprintSoundMulitplier;
    }
    public override void UpdateState()
    {
        base.UpdateState();
        if (inputActions.Movement.Sprint.WasReleasedThisFrame())
        {
            playerController.TransitionToState(playerController.WalkState);
        }
        if (playerController.currentStamina <= 0.0f || inputActions.Movement.Sprint.WasReleasedThisFrame())
        {
            playerController.TransitionToState(playerController.WalkState);
            playerController.staminaDepleted = true;
        }

        if (playerController.Standing) return;

        playerController.currentStamina -= playerController.StaminaReductionPerSecond * Time.deltaTime;

        int occlusionHitCount = 0;
        foreach (var other in Physics.OverlapSphere(playerController.transform.position, soundRange))
        {
            if (other.GetComponent<EnemyController>() != null)
            {
                occlusionHitCount = other.GetComponent<EnemyController>().CheckOcclusion(playerController.transform.position);
                float occludedDistance = (soundRange * 9 - soundRange * occlusionHitCount) / 9;
                if (Vector3.Distance(playerController.transform.position, other.transform.position) <= occludedDistance)
                {
                    other.GetComponent<EnemyController>().HearSound(playerController.gameObject,"Sprint", occlusionHitCount);
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
