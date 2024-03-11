using UnityEngine;
public class PlayerState_Hide : PlayerState_Moving
{
    public override void Enter()
    {
        base.Enter();
        playerController.Crouch(true);
        moveSpeed = playerController.SneakSpeed;
        playerController.currMoveSpeed = moveSpeed;
        playerController.ShowSneakIcon(true);
    }
    public override void OnSneak() { }
    public override void OnSprint() { }
    public override void OnJump() { }
    public override void Blink(Vector3 targetPosition) { }
    public override bool TryPlaceTrap()
    {
        return false;
    }
    public override void Exit()
    {
        base.Exit();
        playerController.Crouch(false);
        playerController.ShowSneakIcon(false);
    }

    public override void Hide()
    {
        Debug.LogWarning("UNHIDE");
        playerController.TransitionToState(playerController.SneakState);
    }

    public override void Snapshot()
    {
        playerController.TransitionToState(playerController.SnapshotState);
    }
}
