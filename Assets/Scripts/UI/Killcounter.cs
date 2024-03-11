using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Killcounter : MonoBehaviour, ISaveable
{
    private List<EnemyController> enemys;
    private TextMeshProUGUI textMeshProComponent;
    private int deadEnemys;

    private void Awake()
    {
        enemys = FindObjectsOfType<EnemyController>(true).ToList();
        textMeshProComponent = GetComponent<TextMeshProUGUI>();
        int aliveEnemys = 0;
        foreach (EnemyController enemy in enemys)
        {
            if (!enemy.Destroyed)
            { 
                aliveEnemys++;
            }
        }
        deadEnemys = enemys.Count - aliveEnemys;
        UpdateText();
    }

    private void Update()
    {
        UpdateText();
    }

    private void UpdateText()
    {
        enemys = FindObjectsOfType<EnemyController>(true).ToList();
        int aliveEnemys = 0;
        foreach (EnemyController enemy in enemys)
        {
            if (!enemy.Destroyed)
            {
                aliveEnemys++;
            }
        }
        deadEnemys = enemys.Count - aliveEnemys;
        textMeshProComponent.text = ( deadEnemys.ToString() + " | " + enemys.Count.ToString() ); 
    }

    private void EnemyDied()
    { 
        UpdateText();
    }

    public void LoadData(GameData data)
    {
        UpdateText();
    }

    public void SaveData(ref GameData data)
    { }
}
