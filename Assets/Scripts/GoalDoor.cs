using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalDoor : MonoBehaviour
{
    [SerializeField]
    private Animation anim;
    private List<EnemyController> enemys;
    private void Start()
    {
        enemys = FindObjectsOfType<EnemyController>(true).ToList();
        if(enemys.Count > 0 )
        {
            foreach(EnemyController enemy in enemys)
            {
                enemy.enemyDied += CheckDoor;
            }
        }
        CheckDoor();
    }

    private void CheckDoor()
    {
        if (enemys.Count == 0 && enemys != null)
        {
            Open();
        }

        if (NoEnemyAlive())
        {
            
            AudioController.Instance.PlayOneShot(AudioController.Instance.exitOpen_ref, GameObject.FindFirstObjectByType<PlayerController>().transform.position);
            Debug.Log("All enemies died. Door '" + gameObject.name + "' opens!");
            Open();
        }
    }

    private void Open()
    {
        if (anim != null)
        {
            anim.Play();
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private bool NoEnemyAlive()
    {
        bool noEnemyAlive = true;
        foreach (EnemyController enemy in enemys)
        {
            if (!enemy.Destroyed)
            {
                noEnemyAlive = false;
            }
        }
        return noEnemyAlive;
    }
}
