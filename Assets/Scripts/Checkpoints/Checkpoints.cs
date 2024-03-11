using System;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    [SerializeField]
    private LayerMask targetLayer;

    private Controls interactionInputAction;

    private bool targetInCollider = false;
    [SerializeField]
    private GameObject buttonPrompt;
    [SerializeField]
    private GameObject enemyAlertedPrompt;
    [SerializeField]
    private Animation savedPromptFading;
    private List<EnemyController> listOfEnemys;
    private bool saveFromMonster = false;

    private void Awake()
    {
        interactionInputAction = new Controls();
        //FindObjectOfType<UIController>().gamePause += OnPause;
    }

    private void OnPause(bool val)
    {
        if (val && targetInCollider)
        {
            buttonPrompt.gameObject.SetActive(true);
        }
        else
        {
            buttonPrompt.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        interactionInputAction.Enable();
    }

    private void OnDisable()
    {
        interactionInputAction.Disable();
    }

    private void Start()
    {
        listOfEnemys = FindAllEnemys();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MathF.Pow(2, other.gameObject.layer) == targetLayer)
        {
            targetInCollider = true;
            buttonPrompt.gameObject.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        saveFromMonster = NoEnemyAlertOrChase();

        if (targetInCollider && interactionInputAction.Movement.Interact.WasPerformedThisFrame()) 
        {
            if (saveFromMonster)
            {
                if(buttonPrompt.activeSelf)
                {
                    buttonPrompt.gameObject.SetActive(false);
                    enemyAlertedPrompt.SetActive(false);
                    AudioController.Instance.PlayOneShot(AudioController.Instance.gameSaved_ref, transform.position);
                    SaveSystem.instance.Save();
                    savedPromptFading.Play("FadeInSaved");
                }
            }
            else
            {
                savedPromptFading.Play("Anim_FadeInEnemyAlerted");
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        targetInCollider = false;
        buttonPrompt.gameObject.SetActive(false);
    }

    public List<EnemyController> FindAllEnemys()
    {
        IEnumerable<EnemyController> list = FindObjectsOfType<EnemyController>(true);
        return new List<EnemyController>(list);
    }

    public bool NoEnemyAlertOrChase()
    {
        bool anyEnemyEnabled = false;

        foreach (var enemy in listOfEnemys)
        {
            if (!enemy.isActiveAndEnabled)
                continue;

            anyEnemyEnabled = true;

            if (enemy.CurrentState.Type != Enemy_StateTypes.INVESTIGATION &&
                enemy.CurrentState.Type != Enemy_StateTypes.CHASE && 
                enemy.CurrentState.Type != Enemy_StateTypes.ALERT)
            {
                return true;
            }
        }

        if(anyEnemyEnabled)
            return false;

        return true;
    }

    public void OnFadeInSavedFinish()
    {
        savedPromptFading.Play("FadeOutSaved");
    }

    public void OnFadeInEnemyAlertedFinished()
    {
        savedPromptFading.Play("Anim_FadeOutEnemyAlerted");
    }
}
