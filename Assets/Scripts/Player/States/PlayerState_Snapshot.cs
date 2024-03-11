using System.Collections;
using UnityEngine;

public class PlayerState_Snapshot : PlayerState
{
    private float timer;
    public override void Enter()
    {
        base.Enter();
        timer = 0f;
        playerController.GetComponent<CameraController>().SwitchTarget(FindNearestEnemy());
    }
    public override void UpdateState()
    {
        base.UpdateState();
        timer += Time.deltaTime;
        if (timer > playerController.SnapshotDuration) 
        {
            playerController.GetComponent<CameraController>().SwitchTarget();
            playerController.TransitionToState(playerController.HideState);
        }
    }
    public override void OnSneak() { }
    public override void OnSprint() { }
    public override void OnJump() { }
    public override void Blink(Vector3 targetPosition) { }
    public override bool TryPlaceTrap()
    {
        return false;
    }
    private GameObject FindNearestEnemy()
    {
        foreach (var other in Physics.OverlapSphere(playerController.transform.position, playerController.SnapshotRange))
        {
            if (other.GetComponent<IDistractable>() != null)
            {
                return other.gameObject;
            }
        }
        return null;
    }
}
