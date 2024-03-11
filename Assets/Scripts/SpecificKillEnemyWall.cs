using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SpecificKillEnemyWall : MonoBehaviour
{
    [Header("Enemies that need to be killed to open the door")]
    [SerializeField]
    private List<EnemyController> enemies;

    [SerializeField]
    private PlayableDirector openDoorAnimation;

    private void Start()
    {
        foreach (EnemyController enemy in enemies)
        {
            enemy.enemyDied += CheckDoor;
        }
        //Debug.Log("Enemies for door: " + enemies);
        CheckDoor();
    }

    private void CheckDoor()
    {
        bool allEnemiesDead = true;
        foreach(EnemyController enemy in enemies)
        {
            if (!enemy.Destroyed)
            {
                //Debug.Log("Found an Enemy that is alive: " + enemy.name);
                allEnemiesDead = false;
                continue;
            }
        }

        if (allEnemiesDead)
        {
            //Debug.Log("All Enemies dead: " + allEnemiesDead.ToString());
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if(openDoorAnimation != null) 
        {
            //Debug.Log("Door animation exists");
            openDoorAnimation.Play();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnOpenDoorAnimationFinished()
    {
        Destroy(gameObject);
    }
}
