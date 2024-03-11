using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class WeakpointTutorialWall : MonoBehaviour
{
    [SerializeField]
    private PlayableDirector destroyAnimation;
    [SerializeField]
    private BoxCollider triggerBox;

    private bool isDestroying = false;
    private UIController uiController;
    private Controls inputActions;

    private void Awake()
    {
        inputActions = new Controls();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void Start()
    {
        uiController = FindObjectOfType(typeof(UIController)).GetComponent<UIController>();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            uiController.ShowKillPrompt(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if(inputActions.Movement.Takedown.WasPerformedThisFrame() && !isDestroying)
        {
            uiController.ShowKillPrompt(false);
            StartDestroyAnimation();
            triggerBox.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiController.ShowKillPrompt(false);
        }
    }

    public void StartDestroyAnimation()
    {
        isDestroying = true;
        destroyAnimation.Play();
    }

    public void OnDestroyFinished()
    {
        Destroy(gameObject);
    }
}
