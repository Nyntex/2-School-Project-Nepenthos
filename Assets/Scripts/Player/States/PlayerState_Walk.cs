using FMOD.Studio;
using UnityEngine;
public class PlayerState_Walk : PlayerState_Moving
{
    private float soundRange;
    public override void Enter()
    {
        base.Enter();
        moveSpeed = playerController.WalkSpeed;
        playerController.currMoveSpeed = moveSpeed;
        if(AudioController.Instance != null ) AudioController.Instance.SetSoundArea(SoundAreaType.PLAYER, (float)PlayerMovingState.WALK);
        float min, max;
        EventDescription description;
        AudioController.Instance.playerFootsteps.getDescription(out description);
        description.getMinMaxDistance(out min, out max);
        soundRange = max * playerController.WalkSoundMulitplier;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (playerController.Standing) return;
        

        int occlusionHitCount = 0; 
        //Debug.Log("Sound Range: " + soundRange);
        foreach (var other in Physics.OverlapSphere(playerController.transform.position, soundRange))
        {
            if (other.GetComponent<EnemyController>() != null)
            {
                //Debug.Log("Enemy in general sound range");
                occlusionHitCount = other.GetComponent<EnemyController>().CheckOcclusion(playerController.transform.position);
                float occludedDistance = (soundRange * 9 - soundRange * occlusionHitCount) / 9;
                if (Vector3.Distance(playerController.transform.position, other.transform.position) <= occludedDistance)
                {
                    other.GetComponent<EnemyController>().HearSound(playerController.gameObject, "Walk", occlusionHitCount);
                }
            }
        }
    }
}