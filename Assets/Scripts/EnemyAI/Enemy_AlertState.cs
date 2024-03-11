using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Enemy_AlertState : EnemyState
{
    //[SerializeField]
    //[Min(0.0f)]
    //private float timeToResetToPatrol = 5.0f;
    //private float currentTimeToResetToPatrol;

    [SerializeField]
    private float percentChanceToWaitAtWaypoint = 0.0f;

    [SerializeField]
    private float timeToWaitAtWaypoint = 2.0f;
    private float currentWaitTime = 0.0f;

    [SerializeField]
    private bool debugLogs = true;

    private bool playerSeenLastFrame = false;

    private void Awake()
    {
        type = Enemy_StateTypes.ALERT;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        HandleAlertness();
        MoveToNextWaypoint();
    }

    public override void Enter()
    {
        //TRACK HOW OFTEN THIS STATE WAS ENTERED
        visionField.distractionSeen += DetectDistraction;
        base.Enter();
        currentWaitTime = 0.0f;
        Debug.Log("Entered Alert State");
        AudioController.Instance.PlayOneShot(AudioController.Instance.enemyAlert_ref, this.transform.position);
        GetComponent<EnemyController>().noiseInstance.setParameterByName("EnemyAlertState", (float)EnemyAlertState.DISTRACTED);
        //Debug.Log(controller.currentWaypointIndex);
        agent.ResetPath();
        currentWaitTime = timeToWaitAtWaypoint;
        SetDestination(controller.Waypoints[controller.currentWaypointIndex].transform.position);
    }

    public override void Exit(Enemy_StateTypes stateType)
    {
        visionField.distractionSeen -= DetectDistraction;
        base.Exit(stateType);
    }

    public override void DetectDistraction(Vector3 distractionPosition)
    {
        if (GetComponent<Enemy_InvestigationState>())
        {
            Exit(Enemy_StateTypes.INVESTIGATION);
            GetComponent<Enemy_InvestigationState>().DetectDistraction(distractionPosition);
        }
    }

    private void HandleAlertness()
    {
        // check if player entered the vision field and wasn't seen last frame
        if (!playerSeenLastFrame && visionField.AnySightSeesTarget())
        {
            //TRACK HOW OFTEN THE PLAYER ENTERED LINE OF SIGHT AND THEREFORE INCREASED THE ALERTNESS
            controller.currentAlertness += controller.ExtraAlertnessIncreaseForPlayerEnterLineOfSight;
        }

        //as long as player remains seen increase alertness
        if (visionField.AnySightSeesTarget())
        {
            controller.currentAlertnessReductionSecond = 0.0f;

            float alertnessRaisePerSecond = 0.0f;
            if (visionField.visionData.targetGameObject == null) return;
            var player = visionField.visionData.targetGameObject.GetComponent<PlayerController>();
            switch (player.CurrentState)
            {
                case PlayerState_Sneak:
                    alertnessRaisePerSecond = controller.CrouchingInVisionAlertnessRaisePerSecond;
                    break;
                case PlayerState_Walk:
                    if (player.Standing)
                        alertnessRaisePerSecond = controller.StandingInVisionAlertnessRaisePerSecond;
                    else
                        alertnessRaisePerSecond = controller.MovingInVisionAlertnessRaisePerSecond;
                    break;
                default:
                    alertnessRaisePerSecond = controller.MovingInVisionAlertnessRaisePerSecond;
                    break;
            }

            float multiplier = visionField.GetVisionFieldAlertnessMultiplier() * (1 + (controller.currentExtraAlertnessMultiplier));
            controller.currentAlertness += alertnessRaisePerSecond * (Time.deltaTime * multiplier);
            if ((visionField.visionData.currentDistanceToTargetPosition < controller.MinPlayerDistanceToInstantFillInnerSight && visionField.TargetInInnerSight()) || (visionField.visionData.currentDistanceToTargetPosition < controller.MinPlayerDistanceToInstantFillOuterSight && visionField.TargetInOuterSight()))
            {
                controller.currentAlertness = controller.MaxAlertness;
            }
        }
        else //if not seeing the player start a timer at which point the alertness gets reduced
        {
            controller.currentAlertnessReductionSecond += Time.deltaTime;
            if (controller.currentAlertnessReductionSecond >= controller.AlertnessReductionAfterSecond)
            {
                controller.currentAlertness -= controller.AlertnessReductionPerSecond * Time.deltaTime;
                if (controller.currentAlertness < 0.0f)
                {
                    controller.currentAlertness = 0.0f;
                    Exit(Enemy_StateTypes.PATROL);
                }

                //if (agent.remainingDistance <= 0.1f)
                //{
                //    currentTimeToResetToPatrol += Time.deltaTime;
                //}
                //if (currentTimeToResetToPatrol >= timeToResetToPatrol)
                //{
                //    Exit(Enemy_StateTypes.PATROL);
                //    controller.currentAlertness = 0.0f;
                //}
            }
        }

        if (controller.currentAlertness >= controller.MaxAlertness && visionField.AnySightSeesTarget())
        {
            controller.currentAlertness = 0.0f;

            controller.currentExtraAlertnessMultiplier += controller.ExtraAlertnessMultiplier;
            if(controller.currentExtraAlertnessMultiplier > controller.MaxExtraAlertnessMultiplier)
                controller.currentExtraAlertnessMultiplier = controller.MaxExtraAlertnessMultiplier;

            Exit(Enemy_StateTypes.CHASE);
        }

        playerSeenLastFrame = visionField.AnySightSeesTarget();
    }

    private void MoveToNextWaypoint()
    {
        if (agent.remainingDistance < 0.1 || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial)
        {
            if (currentWaitTime <= 0.0f)
            {
                if (percentChanceToWaitAtWaypoint >= (float)Random.Range(0, 101))
                {
                    if (debugLogs) Debug.Log("Waiting");
                    currentWaitTime = timeToWaitAtWaypoint;
                }
                else if (controller.Waypoints[controller.currentWaypointIndex].ChanceToWaitHere * 100 >= (float)Random.Range(0, 101))
                {
                    if (debugLogs) Debug.Log("Waiting");
                    currentWaitTime = controller.Waypoints[controller.currentWaypointIndex].TimeToWaitHere;
                }
            }

            if (currentWaitTime > 0.0f)
            {
                currentWaitTime -= Time.deltaTime;
            }

            if (currentWaitTime <= 0.0f)
            {
                if (controller.CurrentState != this)
                {
                    return;
                }
                controller.currentWaypointIndex++;
                if (controller.currentWaypointIndex >= controller.Waypoints.Count)
                {
                    controller.currentWaypointIndex = 0;
                }
                SetDestination(controller.Waypoints[controller.currentWaypointIndex].transform.position);

                if (debugLogs) Debug.Log("Moving To Next Waypoint: " + controller.Waypoints[controller.currentWaypointIndex].ToString());
            }

        }
    }

    public override void HearPlayerMove(GameObject source, string moveType, float soundOcclusionMisses)
    {
        controller.currentAlertnessReductionSecond = 0f;
        switch (moveType) 
        {
            case "Walk":
                controller.currentAlertness += Time.deltaTime * controller.WalkHearSoundAlertness * ((9f - soundOcclusionMisses) / 9f);
                //Debug.Log("Sound Occlusion Misses: " + soundOcclusionMisses);
                break;
            case "Sprint":
                controller.currentAlertness += Time.deltaTime * controller.SprintHearSoundAlertness * ((9f - soundOcclusionMisses) / 9f);
                //Debug.Log("Sound Occlusion Misses: " + soundOcclusionMisses);
                break;
            default:
                Debug.LogError("Player produces sound from an unknown state");
                break;
        }

        if (controller.currentAlertness > controller.MaxAlertness)
        { 
            controller.currentAlertness = controller.MaxAlertness;
            if(controller.GetComponent<Enemy_InvestigationState>() != null) 
            { 
                Exit(Enemy_StateTypes.INVESTIGATION);
                controller.GetComponent<Enemy_InvestigationState>().HearPlayerMove(source, moveType, soundOcclusionMisses);
            }
        }

    }

}
