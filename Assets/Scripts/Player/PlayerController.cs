using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, IDamageable, ISaveable
{
    public Action PlayerDied;

    [SerializeField]
    private float maxHealth;
    public float MaxHealth { get { return maxHealth; } }
    public float currentHealth { get; private set; }
    [SerializeField]
    private float healPerSecond = 10f;

    [Header("Movement")]
    [Space(5)]

    [SerializeField]
    private float walkSpeed;
    public float WalkSpeed => walkSpeed;
    [SerializeField]
    private float walkSoundMulitplier;
    public float WalkSoundMulitplier => walkSoundMulitplier;

    [SerializeField]
    private float sprintSpeed;
    public float SprintSpeed => sprintSpeed;

    [SerializeField]
    private float sprintSoundMulitplier;
    public float SprintSoundMulitplier => sprintSoundMulitplier;

    [SerializeField]
    private float sneakSpeed;
    public float SneakSpeed => sneakSpeed;

    [SerializeField]
    private float movementSmoothing;
    public float MovementSmoothing => movementSmoothing;
    [Space(5)]

    [SerializeField]
    private float jumpStrength;
    public float JumpStrength => jumpStrength;
    [SerializeField]
    private float gravityStrength;
    public float GravityStrength => gravityStrength;
    [SerializeField]
    private float crouchHeight;
    public float CrouchHeight => crouchHeight;
    [SerializeField]
    private float maxStamina = 100;
    public float MaxStamina { get { return maxStamina; } }
    //[HideInInspector]
    public float currentStamina;
    [SerializeField]
    private float staminaReductionPerSecond = 5f;
    public float StaminaReductionPerSecond { get {  return staminaReductionPerSecond; } }
    [SerializeField]
    private float staminaRegenerationPerSecond = 5f;
    public float StaminaRegenerationPerSecond { get { return staminaRegenerationPerSecond; } }

    [Space(5)]
    [Header("Abilities")]
    [Space(5)]
    [SerializeField]
    private float doubleTakedownWindow;
    public float DoubleTakedownWindow => doubleTakedownWindow;
    [SerializeField]
    private float doubleTakedownRange = 5f;
    private bool takeDownPossible = false;
    private bool doubleTakedownPossible = false;
    GameObject takedownTarget = null;
    [Space(5)]
    public GameObject trapPrefab;
    public float trapPlacementRange;
    [SerializeField]
    private int maxTrapsAvailable = 3;
    private int trapsAvailable;
    [Space(5)]
    [SerializeField]
    private float snapshotDuration;
    public float SnapshotDuration => snapshotDuration;
    [SerializeField]
    private float snapshotRange;
    public float SnapshotRange => snapshotRange;
    [Space(5)]
    [HideInInspector]
    public DistractionObject distractionObject;
    [SerializeField]
    private float throwForce;
    public float ThrowForce => throwForce;

    [SerializeField]
    private LayerMask enemyLayer;

    private PlayerState walkState;
    public PlayerState WalkState => walkState;
    private PlayerState sprintState;
    public PlayerState SprintState => sprintState;
    private PlayerState sneakState;
    public PlayerState SneakState => sneakState;
    private PlayerState jumpState;
    public PlayerState JumpState => jumpState;
    private PlayerState hideState;
    public PlayerState HideState => hideState;
    private PlayerState snapshotState;
    public PlayerState SnapshotState => snapshotState;
    private PlayerState currentState;
    public PlayerState CurrentState { get { return currentState; } }
    private PlayerState lastState;
    public PlayerState LastState { get { return lastState; } }

    [HideInInspector]
    public Vector3 currMoveVelocity;
    [HideInInspector]
    public Vector3 moveDampVelocity;
    [HideInInspector]
    public float currMoveSpeed;

    private Controls inputActions;
    public Controls InputActions => inputActions;
    [SerializeField]
    private UIController ui;
    [SerializeField]
    private GameObject collisionObject;

    public GameObject itemInHand;
    public Vector3 pickupOffset = new Vector3(-0.5f, 0.5f, 1.5f);
    private Vector3 positionLastFrame = Vector3.zero;
    private bool standing;
    public bool Standing { get { return standing; } }

    [HideInInspector]
    public bool dead = false;
    private bool damaged;
    private float miniTimer = 0.2f;
    private float currentWidth;
    private float currentHeight;
    private float targetHeight;
    private float targetWidth;
    [HideInInspector]
    public bool staminaDepleted = false;

    [SerializeField]
    private Animator handAnimator;
    public Animator HandAnimator => handAnimator;
    [SerializeField]
    private Animation cameraTakedownAnimation;
    [SerializeField]
    private PlayableDirector armKillVFX;
    [SerializeField]
    private Animator characterAnimation;

    void Awake()
    {
        AudioController.Instance.CreatePlayerSounds(transform);
        armKillVFX.GetComponent<SkinnedMeshRenderer>().material.SetFloat("_Emission", 0f);
        inputActions = new Controls();
        walkState = new PlayerState_Walk();
        walkState.Initialize(this);
        sprintState = new PlayerState_Sprint();
        sprintState.Initialize(this);
        sneakState = new PlayerState_Sneak();
        sneakState.Initialize(this);
        jumpState = new PlayerState_Jump();
        jumpState.Initialize(this);
        hideState = new PlayerState_Hide();
        hideState.Initialize(this);
        snapshotState = new PlayerState_Snapshot();
        snapshotState.Initialize(this);

        currentState = walkState;
        currentState.Enter();

        trapsAvailable = maxTrapsAvailable;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
        currentHeight = GetComponent<CharacterController>().height;
        currentWidth = GetComponent<CharacterController>().radius;
        targetHeight = currentHeight;
        targetWidth = currentWidth;
    }

    private void Start()
    {
        GetComponent<PlayerDeathEffect>().UpdateIntensityByHealth(currentHealth, maxHealth);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;

        HandleInput();
        ChangeHeight();
        HandleAnimation();
        currentState.UpdateState();
        if (currentState != sprintState || Standing)
        {
            currentStamina += Time.deltaTime * staminaRegenerationPerSecond;
            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
                staminaDepleted = false;
            }
        }
        
        if(!damaged)
        {
            HealDamage(healPerSecond * Time.deltaTime);
        }
        if(miniTimer <= 0f)
        {
            damaged = false;
        }
        if(damaged)
        {
            miniTimer -= Time.deltaTime;
        }
        else
        {
            miniTimer = 0.2f;
        }
        standing = Vector3.Distance(positionLastFrame, transform.position) <= 0.1f * Time.deltaTime;
        positionLastFrame = transform.position;
    }

    private void HandleInput()
    {
        if (inputActions.Movement.Sneak.WasPerformedThisFrame())
        {
            currentState.OnSneak();
        }
        if (inputActions.Movement.Sprint.WasPressedThisFrame())
        {
            if(currentStamina > 0.0f && !staminaDepleted)
            { 
                currentState.OnSprint();
            }
        }
        if (inputActions.Movement.Jump.WasPerformedThisFrame())
        {
            currentState.OnJump();
        }
        if (inputActions.Mouse.LeftClick.WasPerformedThisFrame())
        {
            if(inputActions.Mouse.RightHold.ReadValue<float>() > 0f)
            {
                currentState.Snapshot();
            }
            else
            {
                if (distractionObject != null)
                {
                    HandAnimator.SetTrigger("Throw");
                    HandAnimator.SetBool("GrabbedDistractable", false);
                    ui.ShowCrosshair(false);
                }
            }
        }
        if (inputActions.Mouse.RightClick.WasReleasedThisFrame())
        {
            PlaceTrap();
        }
        if (inputActions.Movement.Takedown.WasPerformedThisFrame())
        {
            if(takeDownPossible || doubleTakedownPossible)
            {
                //handAnimator.SetTrigger("Takedown");
                //cameraTakedownAnimation.Play();
                armKillVFX.time = 1.35f;
                armKillVFX.Play();
                
                Debug.Log("TAKEDOWN");
                StartCoroutine(Takedown());
            }
        }
    }

    public void TransitionToState(PlayerState targetState)
    {
        lastState = currentState;
        currentState.Exit();
        currentState = targetState;
        currentState.Enter();
    }

    public void Detected(Vector3 pos)
    {
        float distance = Vector3.Distance(pos,transform.position);
        //GetComponent<PlayerDeathEffect>().UpdateIntensityByDistance(distance);
    }
    public void Detected(bool val)
    {
        if(!val) { GetComponent<PlayerDeathEffect>().Stop(); }
    }
    public void ShowSneakIcon(bool val)
    {
        ui.ShowSneakIcon(val);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        GetComponent<PlayerDeathEffect>().UpdateIntensityByHealth(currentHealth, maxHealth);
        if (currentHealth < 0f)
        {
            //StartCoroutine(Die());
            Die();
        }
        damaged = true;
    }

    public void HealDamage(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        GetComponent<PlayerDeathEffect>().UpdateIntensityByHealth(currentHealth, maxHealth);
        //Debug.Log("Player Controller HealDamage()");
    }

    private void Die()
    {
        Debug.Log("Player Controller Damaged()");
        dead = true;
        SaveSystem.instance.IncreasePlayerDeath();
        SaveSystem.instance.SaveCurrentState();
        AudioController.Instance.PlayOneShot(AudioController.Instance.playerDeath_ref, transform.position);
        //yield return new WaitForSeconds(0.3f);
        PlayerDied.Invoke();
        AudioController.Instance.Pause(true);
        ui.ShowDeathScreen(true);
    }
    public void PickupDistractionObject(DistractionObject target)
    {
        handAnimator.SetTrigger("Grab");
        AudioController.Instance.PlayOneShot(AudioController.Instance.distractionPickup_ref, target.transform.position);
        currentState.OnDistractionObjectPickup(target.GetComponent<DistractionObject>());
    }
    public void OnGrab()
    {
        distractionObject.gameObject.transform.SetParent(transform);
        distractionObject.gameObject.transform.localPosition = pickupOffset;
        distractionObject.gameObject.SetActive(false);
        itemInHand.SetActive(true);
        ui.ShowCrosshair(true);
    }
    public void OnThrow()
    {
        currentState.ThrowDistractionObject();
    }
    public bool IsHidden()
    {
        return currentState == hideState;
    }
    public void Crouch(bool val) // only needed until we have an animated character model... yea... animated character model... sure
    {
        if (val)
        {
            targetHeight = CrouchHeight;
            targetWidth = CrouchHeight / 2f;
        }
        else
        {
            targetHeight = 2f;
            targetWidth = 0.5f;
        }
    }

    private void ChangeHeight()
    {
        currentWidth = Mathf.Lerp(currentWidth, targetWidth, 4f * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, targetHeight, 4f * Time.deltaTime);
        collisionObject.GetComponent<CapsuleCollider>().radius = currentWidth;
        collisionObject.GetComponent<CapsuleCollider>().height = currentHeight;
        GetComponent<CharacterController>().height = currentHeight;
        GetComponent<CharacterController>().radius = currentWidth;
    }

    private void PlaceTrap()
    {
        if(trapsAvailable > 0)
        {
            if (currentState.TryPlaceTrap())
            {
                trapsAvailable--;
            }
        }
    }

    private IEnumerator Takedown()
    {
        if(takeDownPossible)
        {
            AudioController.Instance.PlayOneShot(AudioController.Instance.takedown_ref, transform.position);
            takedownTarget.GetComponentInParent<IDamageable>().TakeDamage(100f);
            takeDownPossible = false;
            yield return new WaitForSeconds(.1f);
            Collider[] objInRange = Physics.OverlapSphere(transform.position, doubleTakedownRange, enemyLayer);
            if (objInRange.Length > 0)
            {
                takedownTarget = objInRange[0].gameObject;
                doubleTakedownPossible = true;
                yield return new WaitForSeconds(doubleTakedownWindow);
                doubleTakedownPossible = false;
                ui.ShowKillPrompt(false);
                takedownTarget = null;
            }
            else
            {
                doubleTakedownPossible = false;
                ui.ShowKillPrompt(false);
                takedownTarget = null;
            }
        }
        else
        {
            takedownTarget.GetComponentInParent<IDamageable>().TakeDamage(100f);
            doubleTakedownPossible = false;
            ui.ShowKillPrompt(false);
            takedownTarget = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            Debug.LogWarning("HIDE");
            currentState.Hide();
        }
        if (other.gameObject.layer == 9 && !(takeDownPossible || doubleTakedownPossible))
        {
            if(other.GetComponent<EnemyController>())
            {
                //Debug.Log("Is Enemy dying: " + other.GetComponent<EnemyController>().Dying);
                if (other.GetComponent<EnemyController>().Dying) return;
                if (other.GetComponent<EnemyController>().CurrentState.Type == Enemy_StateTypes.CHASE) return;

            }
            if (other.GetComponentInParent<EnemyController>())
            {
                //Debug.Log("Is Enemy dying: " + other.GetComponentInParent<EnemyController>().Dying);
                if (other.GetComponentInParent<EnemyController>().Dying) return;
                if (other.GetComponentInParent<EnemyController>().CurrentState.Type == Enemy_StateTypes.CHASE) return;
            }
            ui.ShowKillPrompt(true);
            takeDownPossible = true;
            takedownTarget = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 && currentState == hideState)
        {
            currentState.Hide();
        }
        if (other.gameObject.layer == 9)
        {
            ui.ShowKillPrompt(false);
            takeDownPossible = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * trapPlacementRange));
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, snapshotRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 25);
    }

    public void LoadData(GameData data)
    {
        //Debug.Log(data.playerData.position);
        if (data.playerData.ContainsKey(SceneManager.GetActiveScene().buildIndex))
        {
            PlayerData newData = data.playerData[SceneManager.GetActiveScene().buildIndex];

            if (newData.position == Vector3.zero) { return; }

            //Vector3 direction = data.playerData.position - transform.position;
            if (GetComponent<CharacterController>())
            {

                GetComponent<CharacterController>().enabled = false;
                transform.position = newData.position;
                GetComponent<CharacterController>().enabled = true;
                GetComponent<CameraController>().ChangeRotation(newData.rotation);
                //transform.eulerAngles = new Vector3(0f, newData.rotation.y, 0f);
                //Camera.main.transform.localEulerAngles = new Vector3(newData.rotation.x, 0f, 0f);
            }
            else
            {
                transform.position = newData.position;
                transform.eulerAngles = newData.rotation;
            }
        }
        dead = false;
        //AudioController.Instance.Pause(false);

    }

    public void SaveData(ref GameData data)
    {
        if(data.playerData.ContainsKey(SceneManager.GetActiveScene().buildIndex))
        {
            data.playerData.Remove(SceneManager.GetActiveScene().buildIndex);
        }

        var playerData = new PlayerData(transform.position, new Vector3(Camera.main.transform.localEulerAngles.x, Camera.main.transform.eulerAngles.y, 0f));

        data.playerData.Add(SceneManager.GetActiveScene().buildIndex, playerData);
    }

    private void HandleAnimation()
    {
        if(standing)
        {
            characterAnimation.SetTrigger("Idle");
        }
        else if(currentState == WalkState)
        {
            characterAnimation.SetTrigger("Walk");
        }
        else if(currentState == SprintState) 
        {
            characterAnimation.SetTrigger("Run");
        }
        else if (currentState == SneakState || currentState == HideState)
        {
            characterAnimation.SetTrigger("Crouch");
        }
    }
}