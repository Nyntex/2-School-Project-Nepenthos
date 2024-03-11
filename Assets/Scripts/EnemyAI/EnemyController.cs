using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;
using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine.InputSystem;
using UnityEngine.Timeline;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class EnemyController : MonoBehaviour, IDamageable, IDistractable, ITrappable, ISaveable
{
    #region member
    public Action enemyDied; 

    [SerializeField]
    private float maxHealth;
    private float currentHealth;

    [SerializeField]
    private Animator animator;
    public Animator Animator => animator;
    [SerializeField]
    private PlayableDirector deathTimeline;

    private Vector3 lastFramePosition;

    [Space(10)]
    [Header("Damage Calculation Settings")]
    [Space(5)]
    [SerializeField]
    private float damage;
    public float Damage { get { return damage; } }
    [SerializeField]
    private float a;
    public float A { get { return a; } }
    [SerializeField]
    private float b;
    public float B { get { return b; } }
    [SerializeField]
    private float c;
    public float C { get { return c; } }

    private Enemy_StateTypes initialState = Enemy_StateTypes.PATROL;

    [Space(10)]
    [Header("Waypoint Specifications")]
    [Space(5)]
    [SerializeField]
    private List<Waypoints> waypoints;
    public List<Waypoints> Waypoints { get { return waypoints; } }
    [HideInInspector]
    public int currentWaypointIndex = 0;

    public float minDistanceToAddWaypoint, maxDistanceToAddWaypoint;

    #region Alertness Specifications
    [Space(10)]
    [Header("Alertness Specifications")]
    [Space(5)]
    [SerializeField]
    [Min(0.0f)]
    private float maxAlertness = 100.0f;
    public float MaxAlertness { get { return maxAlertness; } }
    //[HideInInspector]
    public float currentAlertness = 0.0f;

    [SerializeField]
    [Min(0.0f)]
    private float movingInVisionAlertnessRaisePerSecond = 25.0f;
    public float MovingInVisionAlertnessRaisePerSecond { get { return movingInVisionAlertnessRaisePerSecond; } }

    [SerializeField]
    [Min(0.0f)]
    private float standingInVisionAlertnessRaisePerSecond = 25.0f;
    public float StandingInVisionAlertnessRaisePerSecond { get { return standingInVisionAlertnessRaisePerSecond; } }

    [SerializeField]
    [Min(0.0f)]
    private float crouchingInVisionAlertnessRaisePerSecond = 25.0f;
    public float CrouchingInVisionAlertnessRaisePerSecond { get { return crouchingInVisionAlertnessRaisePerSecond; } }

    [SerializeField]
    [Min(0.0f)]
    private float minPlayerDistanceToInstantFillInnerSight = 10.0f;
    public float MinPlayerDistanceToInstantFillInnerSight => minPlayerDistanceToInstantFillInnerSight;

    [SerializeField]
    [Min(0.0f)]
    private float minPlayerDistanceToInstantFillOuterSight = 10.0f;
    public float MinPlayerDistanceToInstantFillOuterSight => minPlayerDistanceToInstantFillOuterSight;

    [SerializeField]
    [Min(0.0f)]
    private float extraAlertnessIncreaseForPlayerEnterLineOfSight = 10.0f;
    public float ExtraAlertnessIncreaseForPlayerEnterLineOfSight { get { return extraAlertnessIncreaseForPlayerEnterLineOfSight; } }

    [SerializeField]
    [Min(0.0f)]
    private float alertnessReductionPerSecond = 5f;
    public float AlertnessReductionPerSecond { get { return alertnessReductionPerSecond; } }

    [SerializeField]
    [Min(0.0f)]
    private float alertnessReductionAfterSecond = 5.0f;
    public float AlertnessReductionAfterSecond { get { return alertnessReductionAfterSecond; } }
    [HideInInspector]
    public float currentAlertnessReductionSecond = 0.0f;

    [SerializeField]
    private float walkHearSoundAlertness;
    public float WalkHearSoundAlertness { get {  return walkHearSoundAlertness; } }

    [SerializeField]
    private float sprintHearSoundAlertness;
    public float SprintHearSoundAlertness { get { return sprintHearSoundAlertness; } }

    [Space(5)]
    [Header("Alertness Multiplier Settings")]
    [Space(5)]
    [SerializeField]
    private float minBaseVisionMultiplier;
    public float MinBaseVisionMultiplier { get { return minBaseVisionMultiplier; } }

    [SerializeField]
    private float maxBaseVisionMultiplier;
    public float MaxBaseVisionMultiplier { get { return maxBaseVisionMultiplier; } }

    [SerializeField]
    [Min(0.0f)]
    private float extraAlertnessMultiplierForChaseEnter = 0.1f;
    public float ExtraAlertnessMultiplier { get { return extraAlertnessMultiplierForChaseEnter; } }
    [HideInInspector]
    public float currentExtraAlertnessMultiplier = 0.0f;

    [SerializeField]
    [Min(0.0f)]
    private float maxExtraAlertnessMultiplierForChaseEnter = 1f;
    public float MaxExtraAlertnessMultiplier { get { return maxExtraAlertnessMultiplierForChaseEnter; } }


    #endregion Alertness Specifications


    [Space(10)]
    [Header("VFX Prefabs")]
    [Space(5)]
    [SerializeField]
    private PlayableSelfDeletingVFX deathVFX;
    #endregion member

    [Space(10)]
    [Header("- - - Please Generate an ID - - -")]
    [Space(10)]
    [SerializeField] private string id;
    #region Generate GUID Button
    [ContextMenu("Generate new GUID")]
    public void GenerateID()
    {
#if UNITY_EDITOR
        if (PrefabStageUtility.GetCurrentPrefabStage() == null)
        {
            id = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
#endif
    }
    [InspectorButton("GenerateID")]
    public bool generate;
    private void OnButtonClicked()
    { GenerateID(); }
    #endregion Generate GUID Button

    private EnemyState currentState;
    public EnemyState CurrentState { get { return currentState; } }

    private bool destroyed = false;
    public bool Destroyed { get { return destroyed; } }
    private bool dying = false;
    public bool Dying => dying;

    public EventInstance noiseInstance;

    private void Awake()
    {
        EnterState(initialState);
        currentHealth = maxHealth;
        currentExtraAlertnessMultiplier = 0.0f;
        currentAlertness = 0.0f;

    }

    private void Start()
    {
        foreach (var component in GetComponents<EnemyState>())
        {
            component.controller = this;
        }
        noiseInstance = AudioController.Instance.CreateEventInstance(AudioController.Instance.enemyNoise_ref);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(noiseInstance, this.transform);
        noiseInstance.start();
        lastFramePosition = transform.position;
    }

    public void HealDamage(float amount)
    {
        if (currentHealth > 0)
        {
            currentHealth += amount;
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (currentHealth > 0)
        {
            Debug.Log(currentHealth);
            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                dying = true;
                GetComponent<NavMeshAgent>().ResetPath();
                //animator.SetTrigger("Die");
                deathTimeline.Play();
                StartCoroutine(DelayedDeath((float)deathTimeline.duration));

                //Die();
            }
        }
    }

    public void EnterState(Enemy_StateTypes stateType)
    {
        switch(stateType) 
        {
            case Enemy_StateTypes.PATROL:
                CheckAndSwitchState<Enemy_PatrolState>(stateType);
                break;
            case Enemy_StateTypes.ALERT:
                CheckAndSwitchState<Enemy_AlertState>(stateType);
                SaveSystem.instance.GetTrackingData().enemiesAlerted++;
                break;
            case Enemy_StateTypes.INVESTIGATION:
                CheckAndSwitchState<Enemy_InvestigationState>(stateType);
                break;
            case Enemy_StateTypes.CHASE:
                CheckAndSwitchState<Enemy_ChaseState>(stateType);
                SaveSystem.instance.GetTrackingData().enemiesAggressive++;
                break;
            case Enemy_StateTypes.NONE:
            default:
                Debug.LogError("No valid state given");
                break;
        }
        currentState.UpdateState();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        HandleAnimation();
        if (dying) return;
        currentState.UpdateState();
    }

    private void Die()
    {
        destroyed = true;
        AudioController.Instance.ClearEventInstance(noiseInstance);
        AudioController.Instance.PlayOneShot(AudioController.Instance.enemyDeath_ref, transform.position);
        Debug.Log("Enemy died!");
        //PlayableSelfDeletingVFX instance = Instantiate(deathVFX, transform.position, Quaternion.identity);
        //instance.transform.SetParent(null);
        //instance.Play();
        gameObject.SetActive(false);
        enemyDied.Invoke();
    }

    private void CheckAndSwitchState<T>(Enemy_StateTypes stateType) where T : EnemyState
    {
        if (GetComponent<T>())
        {
            currentState = GetComponent<T>();
            if(GetComponent<T>().controller == null) 
            {
                GetComponent<T>().controller = this;
            }
            currentState.Enter();
        }
        else
        {
            Debug.LogError("MISSING"+ GetComponent<T>().ToString() + "STATE!!");
        }
    }

    public void Distract(Vector3 sourcePosition) => currentState.DetectDistraction(sourcePosition);

    public void GetTrapped()
    {
        Debug.Log("Trapped");
        GetComponent<NavMeshAgent>().isStopped = true;
    }

    public void GetReleased()
    {
        GetComponent<NavMeshAgent>().isStopped = false;
    }

    public void LoadData(GameData data)
    {
        if(data.enemyDataDictionary.ContainsKey(id))
        {
            
            gameObject.SetActive(true);
            EnemyData enemyData = data.enemyDataDictionary[id];
            transform.position = enemyData.position;
            transform.rotation = enemyData.rotation;
            currentHealth = enemyData.health;
            currentWaypointIndex = enemyData.waypointIndex;
            currentExtraAlertnessMultiplier = enemyData.alertnessSpeed;
            
            currentState.Exit((Enemy_StateTypes)enemyData.stateType);

            destroyed = enemyData.destroyed;
            if (destroyed)
            {
                gameObject.SetActive(false);
                AudioController.Instance.ClearEventInstance(noiseInstance);
            }
            
        }
    }

    public void SaveData(ref GameData data)
    {
        
        if (data.enemyDataDictionary.ContainsKey(id))
        {
            data.enemyDataDictionary.Remove(id);
        }

        EnemyData newData = new EnemyData(transform.position, transform.rotation, (int)currentState.Type, currentHealth, 
                                            currentWaypointIndex, destroyed, currentExtraAlertnessMultiplier);
        
        data.enemyDataDictionary.Add(id, newData);
    }

    public void HearSound(GameObject source, string stateType, float soundOcclusionHits) => currentState.HearPlayerMove(source, stateType, soundOcclusionHits);

    #region SoundOcclusionCheck
    public int CheckOcclusion(Vector3 pos)
    {
        int result = 0;
        Vector3 sound = pos;
        Vector3 listener = transform.position;
        Vector3 soundLeft = FindPoint(sound, listener, 2, true);
        Vector3 soundRight = FindPoint(sound, listener, 2, false);
        Vector3 listenerLeft = FindPoint(listener, sound, 2, true);
        Vector3 listenerRight = FindPoint(listener, sound, 2, false);

        result += CastLine(soundLeft, listenerLeft);
        result += CastLine(soundLeft, listener);
        result += CastLine(soundLeft, listenerRight);

        result += CastLine(sound, listenerLeft);
        result += CastLine(sound, listener);
        result += CastLine(sound, listenerRight);

        result += CastLine(soundRight, listenerLeft);
        result += CastLine(soundRight, listener);
        result += CastLine(soundRight, listenerRight);

        return result;
    }
    private Vector3 FindPoint(Vector3 castSource, Vector3 castTarget, float castSpread, bool goLeft)
    {
        float x;
        float z;
        float n = Vector3.Distance(new Vector3(castSource.x, 0f, castSource.z), new Vector3(castTarget.x, 0f, castTarget.z));
        float mn = (castSpread / n);
        if (goLeft)
        {
            x = castSource.x + (mn * (castSource.z - castTarget.z));
            z = castSource.z - (mn * (castSource.x - castTarget.x));
        }
        else
        {
            x = castSource.x - (mn * (castSource.z - castTarget.z));
            z = castSource.z + (mn * (castSource.x - castTarget.x));
        }
        return new Vector3(x, castSource.y, z);
    }

    private int CastLine(Vector3 castSource, Vector3 castTarget)
    {
        RaycastHit hit;
        Physics.Linecast(castSource, castTarget, out hit, LayerMask.GetMask("Ground", "Wall", "Obstacle", "HidingSpot"));
        if (hit.collider)
        {
            UnityEngine.Debug.DrawLine(castSource, castTarget, Color.red);
            return 1;
        }
        else
        {
            UnityEngine.Debug.DrawLine(castSource, castTarget, Color.blue);
            return 0;
        }
    }
    #endregion SoundOcclusionCheck

    private IEnumerator DelayedDeath(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Die();
    }

    private void HandleAnimation()
    {
        animator.SetBool("Walking", !IsStanding());
        lastFramePosition = transform.position;
        if(currentHealth <= 0f)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("KILLMEALREADY"))
            {
                Die();
            }
        }
    }

    public bool IsStanding()
    {
        return Vector3.Distance(lastFramePosition, transform.position) <= 0.1f * Time.deltaTime;
    }
}


