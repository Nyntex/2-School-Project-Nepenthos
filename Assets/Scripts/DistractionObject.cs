using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

[RequireComponent(typeof(StudioEventEmitter))]
public class DistractionObject : MonoBehaviour, ISaveable
{
    [SerializeField][Min(0.1f)]
    private float pickupRange;

    [SerializeField] [Min(0.1f)]
    private float distractionRange;

    [Space(10)]
    [Header("- - - Do not generate ID in Prefab - - - Please Generate an ID - - -")]
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

    private Rigidbody rb;
    [HideInInspector]
    public bool isDistracting = false;

    private Controls inputActions;
    [SerializeField]
    private WorldPrompt prompt;
    private PlayerController playerController;
    private StudioEventEmitter emitter;

    public Material outlineMaterial;
    [SerializeField]
    private MeshRenderer meshRenderer;
    private Material[] materials;
    [SerializeField]
    private GameObject effects;
    [SerializeField]
    private GameObject seedEffect;

    [SerializeField]
    private PlayableDirector plantGrowTimeline;
    [SerializeField]
    private PlayableDirector plantShrinkTimeline;
    private bool showPrompt = false;
    public enum DistractionObjectState
    {
        NONE = 0,
        INTERACTABLE = 10,
        PICKEDUP = 20,
        GROWING = 30,
        SHRINKING = 40,
    }
    private DistractionObjectState objState;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
        inputActions = new Controls();
        //prompt = GetComponentInChildren<WorldPrompt>(true);
        materials = meshRenderer.materials;
        objState = DistractionObjectState.INTERACTABLE;
        //FindObjectOfType<UIController>().gamePause += OnPause;
    }
    private void Start()
    {
        rb.isKinematic = true;
        effects.SetActive(false);
        emitter = AudioController.Instance.InitializeEventEmitter(AudioController.Instance.distractionEmission_ref, this.gameObject);
        emitter.Play();
        playerController = FindObjectOfType<PlayerController>();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    private void Update()
    {
        switch (objState)
        {
            case DistractionObjectState.INTERACTABLE:
                if (playerController.distractionObject != null) return;
                if (prompt.isActiveAndEnabled)
                {
                    if (inputActions.Movement.Interact.WasPerformedThisFrame())
                    {
                        PickUpObject();
                    }
                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;
        if (playerController.distractionObject != null) return;
        if (objState != DistractionObjectState.INTERACTABLE) return;
        if (IsOtherObjectPickupable()) return;
        PromptPickup(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<PlayerController>()) return;
        PromptPickup(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer != 6) return; 
        AudioController.Instance.PlayOneShot(AudioController.Instance.distractionImpact_ref, transform.position);
        float min, max;
        emitter.EventDescription.getMinMaxDistance(out min,out max);
        gameObject.GetComponent<SphereCollider>().enabled = true;
        int occlusionHitCount = 0;
        
        foreach (var other in Physics.OverlapSphere(transform.position, max))
        {
            if (other.GetComponent<EnemyController>() != null)
            {
                occlusionHitCount = other.GetComponent<EnemyController>().CheckOcclusion(transform.position);
                float occludedDistance = (max * 9 - max * occlusionHitCount)/9;
                if(Vector3.Distance(transform.position, other.transform.position) <= occludedDistance)
                {
                    other.GetComponent<IDistractable>().Distract(transform.position);
                }
            }
            else if (other.GetComponent<IDistractable>() != null)
            {
                other.GetComponent<IDistractable>().Distract(transform.position);
            }
        }

        foreach(var other in Physics.OverlapSphere(transform.position, 0.25f))
        {
            if(other.GetComponent<VisionField>()) 
            { 
                isDistracting = true;  
            }
        }
        emitter.Play();
        objState = DistractionObjectState.GROWING;
        Debug.LogWarning("START GROWING");
        rb.isKinematic = true;
        meshRenderer.enabled = false;
        effects.SetActive(true);
        plantGrowTimeline.Play();
    }

    private void PickUpObject()
    {
        if(playerController == null) 
            playerController = FindObjectOfType<PlayerController>();
        playerController.PickupDistractionObject(this);
        if(emitter == null)
            emitter = AudioController.Instance.InitializeEventEmitter(AudioController.Instance.distractionEmission_ref, this.gameObject);
        emitter.Stop();
        PromptPickup(false);
        seedEffect.SetActive(false);
        objState = DistractionObjectState.PICKEDUP;
        playerController.HandAnimator.SetBool("GrabbedDistractable", true);
    }

    public void Throw(Vector3 direction, float force)
    {
        rb.isKinematic = false;
        rb.AddForce(direction*force, ForceMode.Impulse);
        gameObject.GetComponent<SphereCollider>().enabled = false;
        transform.SetParent(transform.root);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distractionRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere (transform.position, pickupRange);
    }

    private void OnValidate()
    {
        GetComponent<SphereCollider>().radius = pickupRange/transform.localScale.x;
    }

    public void LoadData(GameData data)
    {
        //Debug.Log("Loading: " + gameObject.name);
        if(data.distractionObjectDataDictionary.ContainsKey(id))
        {
            DistractionObjectData dat = data.distractionObjectDataDictionary[id];
            if(dat.position != null)
                transform.position = dat.position;
            //Debug.Log("Position: " + transform.position);
            if(dat.rotation != null)
                transform.rotation = dat.rotation;
            //Debug.Log("Rotation: " + transform.rotation);
            objState = dat.state;
            //Debug.Log("State: " + objState);
            if (dat.state == DistractionObjectState.PICKEDUP)
                PickUpObject();
        }
    }

    public void SaveData(ref GameData data)
    {
        if(data.distractionObjectDataDictionary.ContainsKey(id))
        {
            data.distractionObjectDataDictionary.Remove(id);
        }

        data.distractionObjectDataDictionary.Add(id, new DistractionObjectData(transform.position, transform.rotation, objState));
    }

    private void PromptPickup(bool val)
    {
        showPrompt = val;
        prompt.gameObject.SetActive(val);
        if (val)
        {
            materials[1] = outlineMaterial;
        }
        else
        {
            materials[1] = materials[0];
        }
        meshRenderer.SetMaterials(materials.ToList<Material>());
    }

    public void OnGrowFinished()
    {
        objState = DistractionObjectState.SHRINKING;
        plantShrinkTimeline.Play();
        Debug.LogWarning("GROW FINISHED");
    }
    public void OnShrinkFinished()
    {
        objState = DistractionObjectState.INTERACTABLE;
        Debug.LogWarning("SHRINK FINISHED");
        foreach (var other in Physics.OverlapSphere(transform.position, pickupRange))
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                PromptPickup(true);
            }
        }
        effects.SetActive(false);
        meshRenderer.enabled = true;
        seedEffect.SetActive(true);
    }

    private void OnPause(bool val)
    {
        if(val && showPrompt) 
        {
            //canvas.gameObject.SetActive(true);
        }
        else
        {
            //canvas.gameObject.SetActive(false);
        }
    }

    private bool IsOtherObjectPickupable()
    {
        float min, max;
        emitter.EventDescription.getMinMaxDistance(out min, out max);
        foreach (var obj in Physics.OverlapSphere(transform.position, max))
        {
            if (obj.GetComponent<DistractionObject>())
            {
                if (obj.GetComponent<DistractionObject>().IsShowingPickupPrompt())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsShowingPickupPrompt()
    {
        return prompt.isActiveAndEnabled;
    }
}
