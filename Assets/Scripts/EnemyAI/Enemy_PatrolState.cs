using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy_PatrolState : EnemyState
{
    [SerializeField]
    private float percentChanceToWaitAtWaypoint = 5.0f;

    [SerializeField]
    private float timeToWaitAtWaypoint = 5.0f;
    private float currentWaitTime = 0.0f;

    [SerializeField]
    private bool debugLogs = false;


    private void Awake()
    {
        type = Enemy_StateTypes.PATROL;
    }

    public override void Enter()
    {
        visionField.distractionSeen += DetectDistraction;
        base.Enter();

        if (debugLogs) Debug.Log("Entered State: " + type.ToString());

        

        if (controller.Waypoints.Count <= 1)
        {
            Debug.LogError("Not Enough Waypoints!!");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        GetComponent<EnemyController>().noiseInstance.setParameterByName("EnemyAlertState", (float)EnemyAlertState.ROAMING);
        //Debug.Log(controller.currentWaypointIndex);
        agent.ResetPath();
        currentWaitTime = timeToWaitAtWaypoint;
        SetDestination(controller.Waypoints[controller.currentWaypointIndex].transform.position);
        //Debug.Log(agent.pathStatus);
        //Debug.Log(agent.remainingDistance);

    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (visionField == null)
        {
            Debug.LogError("Vision Field was removed from: " + ToString());
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (type != Enemy_StateTypes.NONE && controller.Waypoints.Count > 1)
        {
            MoveToNextWaypoint();
            DetectPlayer();
        }

        //Debug.Log(currentWaypointIndex);
    }

    private void MoveToNextWaypoint()
    {
        if (agent.remainingDistance < 0.1 || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid || agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial)
        {
            if (currentWaitTime <= 0.0f)
            {
                if(percentChanceToWaitAtWaypoint >= (float)Random.Range(0, 101))
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
                if(controller.currentWaypointIndex >= controller.Waypoints.Count)
                {
                    controller.currentWaypointIndex = 0;
                }
                SetDestination(controller.Waypoints[controller.currentWaypointIndex].transform.position);

                if (debugLogs) Debug.Log("Moving To Next Waypoint: " + controller.Waypoints[controller.currentWaypointIndex].ToString());
            }

        }
    }

    public override void Exit(Enemy_StateTypes stateType)
    {
        visionField.distractionSeen -= DetectDistraction;
        base.Exit(stateType);
    }

    protected override void DetectPlayer()
    {
        if (visionField.AnySightSeesTarget())
        {
            Exit(Enemy_StateTypes.ALERT);
        }
    }

    public override void DetectDistraction(Vector3 distractionPosition)
    {
        if(GetComponent<Enemy_InvestigationState>()) 
        {
            Exit(Enemy_StateTypes.INVESTIGATION);
            GetComponent<Enemy_InvestigationState>().DetectDistraction(distractionPosition);
        }
    }

    public override void HearPlayerMove(GameObject source, string moveType, float soundOcclusionHits)
    {
        Debug.Log("Heared Player (while in Patrol State)");
        if (GetComponent<Enemy_AlertState>())
        {
            GetComponent<Enemy_AlertState>().HearPlayerMove(source, moveType, soundOcclusionHits);
            Exit(Enemy_StateTypes.ALERT);
        }
    }

}
