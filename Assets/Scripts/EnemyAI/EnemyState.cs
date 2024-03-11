using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyController), typeof(NavMeshAgent))]
public class EnemyState : MonoBehaviour, IState
{
    [SerializeField]
    protected NavMeshAgent agent;
    protected float agentBaseSpeed;

    [SerializeField]
    protected float movementSpeed;

    [SerializeField]
    protected EnemyVisionField visionField;

    [HideInInspector]
    public EnemyController controller;

    [SerializeField]
    protected Material eyesMaterial;

    [SerializeField]
    [ColorUsage(true, true)]
    private Color eyeColor;

    protected Enemy_StateTypes type = Enemy_StateTypes.NONE;
    public Enemy_StateTypes Type { get { return type; } }

    private void Awake()
    {
        agentBaseSpeed = agent.speed;
        controller = GetComponent<EnemyController>();
    }

    public virtual void Enter()
    {
        if (controller == null)
        {
            controller = GetComponent<EnemyController>();
            if (controller == null)
            {
                Debug.LogError("ENEMY HAS NO CONTROLLER?? WHAT HAPPENED?");
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError(gameObject.ToString() + " has no NavMeshAgent!!");

#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            agentBaseSpeed = agent.speed;
        }
        agent.speed = movementSpeed;

        eyesMaterial.SetColor("_EyeColorDark", eyeColor);
    }

    public virtual void UpdateState()
    {
        DamageSeenPlayer();
    }

    public virtual void Exit(Enemy_StateTypes stateType)
    { 
        agent.speed = agentBaseSpeed;

        if(GetComponent<EnemyController>())
        {
            GetComponent<EnemyController>().EnterState(stateType);
        }
    }

    public virtual void DetectDistraction(Vector3 distractionPosition)
    { }

    protected virtual void DetectPlayer()
    { }

    protected virtual void SetDestination(Vector3 targetLocation)
    {
        agent.SetDestination(targetLocation);
    }

    public virtual void HearPlayerMove(GameObject source, string stateType, float soundOcclusionHits)
    { }

    protected virtual void DamageSeenPlayer()
    { 
        if(visionField.AnySightSeesTarget() && visionField.visionData.targetGameObject.GetComponent<PlayerController>())
        {
            float x = Vector3.Distance(visionField.visionData.targetGameObject.transform.position, transform.position);
            float maxVisionRange = Mathf.Max(visionField.InnerVisionRange, visionField.OuterVisionRange - visionField.DistanceToStartSeeing);
            x = Mathf.Abs(x / maxVisionRange - 1f);
            //Debug.Log("X: " + x);
            //Debug.Log("Multiplier: " + (Mathf.Pow(controller.A, x - controller.B) + controller.C));
            float damage = controller.Damage * (Mathf.Pow(controller.A,x-controller.B) + controller.C);

            visionField.visionData.targetGameObject.GetComponent<PlayerController>().TakeDamage(damage * Time.deltaTime);
        }
    }
}
