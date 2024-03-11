using FMODUnity;
using UnityEngine;

public class Enemy_InvestigationState : EnemyState
{
    [SerializeField]
    private float alertnessMultiplier = 1.0f;

    [SerializeField]
    private float timeUntilResetToAlert = 5.0f;
    private float currentTimeUntilResetToAlert;

    [SerializeField]
    private EventReference distractactionSound;

    private bool playerSeenLastFrame = false;

    private void Awake()
    {
        type = Enemy_StateTypes.INVESTIGATION;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (!playerSeenLastFrame && visionField.AnySightSeesTarget())
        {
            //TRACK HOW OFTEN THE PLAYER ENTERED LINE OF SIGHT AND THEREFORE INCREASED THE ALERTNESS
            controller.currentAlertness += controller.ExtraAlertnessIncreaseForPlayerEnterLineOfSight * alertnessMultiplier;
        }

        //as long as player remains seen increase alertness
        if (visionField.AnySightSeesTarget())
        {
            currentTimeUntilResetToAlert = 0.0f;

            float alertnessRaisePerSecond = 0.0f;
            var player = visionField.visionData.targetGameObject.GetComponent<PlayerController>();
            switch (player.CurrentState)
            {
                case PlayerState_Walk:
                    if (player.Standing)
                        alertnessRaisePerSecond = controller.StandingInVisionAlertnessRaisePerSecond;
                    break;
                case PlayerState_Sneak:
                    alertnessRaisePerSecond = controller.CrouchingInVisionAlertnessRaisePerSecond;
                    break;
                default:
                    alertnessRaisePerSecond = controller.MovingInVisionAlertnessRaisePerSecond;
                    break;
            }

            float multiplier = visionField.GetVisionFieldAlertnessMultiplier() * (1 + (controller.currentExtraAlertnessMultiplier + alertnessMultiplier));
            controller.currentAlertness += alertnessRaisePerSecond * (Time.deltaTime * multiplier);
            SetDestination(visionField.visionData.targetPositionFromLastDetection);
            if ((visionField.visionData.currentDistanceToTargetPosition < controller.MinPlayerDistanceToInstantFillInnerSight && visionField.TargetInInnerSight() )|| (visionField.visionData.currentDistanceToTargetPosition < controller.MinPlayerDistanceToInstantFillOuterSight && visionField.TargetInOuterSight()))
            {
                controller.currentAlertness = controller.MaxAlertness;
            }
        }
        else if(agent.remainingDistance <= 0.5f || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial)//if not seeing the player start a timer at which point he goes back into alert state
        {
            currentTimeUntilResetToAlert += Time.deltaTime;
            if (currentTimeUntilResetToAlert >= timeUntilResetToAlert)
            {
                Exit(Enemy_StateTypes.ALERT);
                currentTimeUntilResetToAlert = 0.0f;
            }
        }

        if (controller.currentAlertness >= controller.MaxAlertness && visionField.AnySightSeesTarget())
        {
            Exit(Enemy_StateTypes.CHASE);
            controller.currentExtraAlertnessMultiplier += controller.ExtraAlertnessMultiplier;
            if (controller.currentExtraAlertnessMultiplier > controller.MaxExtraAlertnessMultiplier)
                controller.currentExtraAlertnessMultiplier = controller.MaxExtraAlertnessMultiplier;

            controller.currentAlertness = 0.0f;
            currentTimeUntilResetToAlert = 0.0f;
        }

        playerSeenLastFrame = visionField.AnySightSeesTarget();
    }

    public override void Enter()
    {
        //TRACK HOW OFTEN THIS STATE WAS ENTERED
        visionField.distractionSeen += DetectDistraction;
        base.Enter();
        Debug.Log("Entered Investigation State");

        GetComponent<EnemyController>().noiseInstance.setParameterByName("EnemyAlertState", (float)EnemyAlertState.ROAMING);
    }

    public override void Exit(Enemy_StateTypes stateType)
    {
        visionField.distractionSeen -= DetectDistraction;
        base.Exit(stateType);
    }

    public override void DetectDistraction(Vector3 distractionPosition)
    {
        controller.currentAlertness += controller.ExtraAlertnessIncreaseForPlayerEnterLineOfSight * alertnessMultiplier;
        if (!visionField.AnySightSeesTarget())
        {
            AudioController.Instance.PlayOneShot(distractactionSound, transform.position);
            agent.SetDestination(distractionPosition);
        }
        else
        {
            Exit(Enemy_StateTypes.CHASE);
        }
        CheckToAddWaypoint(distractionPosition);
    }

    private void CheckToAddWaypoint(Vector3 position)
    {
        Vector3 original = agent.steeringTarget;

    }

    public override void HearPlayerMove(GameObject source, string stateType, float soundOcclusionHits)
    {
        switch (stateType)
        {
            case "Walk":
                controller.currentAlertness += Time.deltaTime * controller.WalkHearSoundAlertness * (soundOcclusionHits / 9f);
                break;
            case "Sprint":
                controller.currentAlertness += Time.deltaTime * controller.SprintHearSoundAlertness * (soundOcclusionHits / 9f);
                SetDestination(source.transform.position);
                break;
            default:
                Debug.LogError("Player produces sound from an unknown state");
                break;
        }

        if(controller.currentAlertness > controller.MaxAlertness)
        {
            controller.currentAlertness = controller.MaxAlertness;
            SetDestination(source.transform.position);
        }
        
    }
}
