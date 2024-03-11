using UnityEngine;

public class PlayerState_Sneak : PlayerState_Moving
{
    public override void Enter()
    {
        base.Enter();
        moveSpeed = playerController.SneakSpeed;
        playerController.currMoveSpeed = moveSpeed;
        playerController.ShowSneakIcon(true);
        playerController.Crouch(true);
        if(playerController.LastState != playerController.HideState)
        {
            Debug.Log(playerController.LastState);
            AudioController.Instance.PlayOneShot(AudioController.Instance.playerCrouch_ref, playerController.transform.position);
        }
        AudioController.Instance.SetSoundArea(SoundAreaType.PLAYER, (float)PlayerMovingState.CROUCH);
    }

    public override void Exit()
    {
        base.Exit();
        playerController.Crouch(false);
        playerController.ShowSneakIcon(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void OnSneak()
    {
        Ray ray = new Ray(playerController.gameObject.transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);
            if (hit.collider.gameObject.layer == 6 || hit.collider.gameObject.layer == 12)
            {
                if (Vector3.Distance(playerController.gameObject.transform.position, hit.point) <= 2f)
                {
                    return;
                }
            }
        }
        playerController.TransitionToState(playerController.WalkState);
    }
    public override void OnSprint()
    {
        Ray ray = new Ray(playerController.gameObject.transform.position, Vector3.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 1f);
            if (hit.collider.gameObject.layer == 6)
            {
                if (Vector3.Distance(playerController.gameObject.transform.position, hit.point) <= 2f)
                {
                    return;
                }
            }
        }
        playerController.TransitionToState(playerController.SprintState);
    }
}
