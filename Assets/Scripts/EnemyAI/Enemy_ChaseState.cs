using System;
using UnityEngine;

public class Enemy_ChaseState : EnemyState
{
    [SerializeField]
    [Min(0.0f)]
    private float timeUntilPlayerLost = 5.0f;
    private float currentTimeUntilPlayerLost = 0.0f;
    private PlayerController playerController;

    private void Awake()
    {
        type = Enemy_StateTypes.CHASE;
        currentTimeUntilPlayerLost = 0.0f;
    }

    public override void Enter()
    {
        //TRACK HOW OFTEN THIS STATE WAS ENTERED (ENEMY_CHASESTATE_ENTERED)
        base.Enter();
        Debug.Log("Entered Chase State");
        currentTimeUntilPlayerLost = 0.0f;

        GetComponent<EnemyController>().noiseInstance.setParameterByName("EnemyAlertState", (float)EnemyAlertState.AGGRESSIVE);
        playerController = FindObjectOfType<PlayerController>();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (visionField.AnySightSeesTarget())
        {
            playerController.Detected(transform.position);
            DetectPlayer();
        }
        else
        {
            PlayerOutOfSight();
        }
        if(agent.remainingDistance > 0.0f)
        {
            agent.isStopped = false;
        }
        else
        {
            //Debug.Log(Vector3.Distance(transform.position, visionField.visionData.targetGameObject.transform.position));
            agent.isStopped = true;
        }

    }

    private void PlayerOutOfSight()
    {
        //Debug.Log("Not Seeing player: " +  currentTimeUntilPlayerLost.ToString());
        if(visionField.visionData.targetGameObject == null) 
        {
            Debug.LogWarning("Vision Field has no Information about the Player, how did I even enter this state?");
            return;
        }
        SetDestination(visionField.visionData.targetGameObject.transform.position);
        currentTimeUntilPlayerLost += Time.deltaTime;
        if (currentTimeUntilPlayerLost >= timeUntilPlayerLost)
        {
            Debug.Log("Exited Chase State to Investigation State");
            playerController.Detected(false);
            Exit(Enemy_StateTypes.INVESTIGATION);
        }

    }
    /*
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(other.GetComponent<IDamageable>() != null)
            { 
                other.GetComponent<IDamageable>().TakeDamage(100000.0f);
            }
        }
    }
    */
    protected override void DetectPlayer()
    {
        currentTimeUntilPlayerLost = 0.0f;
        SetDestination(playerController.transform.position);
    }


}
