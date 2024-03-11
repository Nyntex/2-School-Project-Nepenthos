using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinScreenData : MonoBehaviour
{
    #region exposed stats
    [Space(5)]
    [Header("Monsters Alerted")]
    [Space(5)]
    [SerializeField]
    private int monsterAlertVeryBad = 0;
    [SerializeField]
    private int monsterAlertBad = 0;
    [SerializeField]
    private int monsterAlertGood = 0;
    [SerializeField]
    private int monsterAlertVeryGood = 0;

    [Space(10)]
    [Header("Monsters Aggressive")]
    [Space(5)]
    [SerializeField]
    private int monsterAggressiveVeryBad = 0;
    [SerializeField]
    private int monsterAggressiveBad = 0;
    [SerializeField]
    private int monsterAggressiveGood = 0;
    [SerializeField]
    private int monsterAggressiveVeryGood = 0;

    [Space(10)]
    [Header("Player Deaths")]
    [Space(5)]
    [SerializeField]
    private int playerDeathsVeryBad = 0;
    [SerializeField]
    private int playerDeathsBad = 0;
    [SerializeField]
    private int playerDeathsGood = 0;
    [SerializeField]
    private int playerDeathsVeryGood = 0;

    [Space(10)]
    [Header("Time Completion (in Seconds)")]
    [Space(5)]
    [SerializeField]
    private int timeVeryBad = 0;
    [SerializeField]
    private int timeBad = 0;
    [SerializeField]
    private int timeGood = 0;
    [SerializeField]
    private int timeVeryGood = 0;

    [Space(10)]
    [Header("Grade")]
    [Space(5)]
    [SerializeField]
    private int lowestGrade = 0;
    [SerializeField]
    private int mediumGrade = 0;
    [SerializeField]
    private int highestGrade = 0;

    [Space(10)]
    [Header("Points")]
    [Space(5)]
    [SerializeField]
    private int veryBadPoints = -2;
    [SerializeField]
    private int badPoints = -1;
    [SerializeField]
    private int goodPoints = 1;
    [SerializeField]
    private int veryGoodPoints = 2;

    [Space(10)]
    [Header("Grade Rewards")]
    [Space(5)]
    [SerializeField]
    private GameObject lowestGradeRewardObject;
    [SerializeField]
    private GameObject mediumGradeRewardObject;
    [SerializeField]
    private GameObject highestGradeRewardObject;

    #endregion exposed stats

    [SerializeField]
    private TextMeshProUGUI playerDeathsText;
    [SerializeField]
    private TextMeshProUGUI enemiesAlertedText;
    [SerializeField]
    private TextMeshProUGUI enemiesAggressiveText;
    [SerializeField]
    private TextMeshProUGUI completionTimeText;

    private void OnEnable()
    {
        TrackingData data = SaveSystem.instance.GetTrackingData();
        int playerDeathAmount = data.playerDeaths;
        int enemiesAlerted = data.enemiesAlerted;
        int enemiesAggressive = data.enemiesAggressive;
        float completionTime = data.completionTime;

        int minutes = (int)(completionTime / 60f);
        int seconds = (int)(completionTime - (minutes * 60));

        playerDeathsText.text = playerDeathAmount.ToString();
        enemiesAlertedText.text = enemiesAlerted.ToString();
        enemiesAggressiveText.text = enemiesAggressive.ToString();
        completionTimeText.text = minutes.ToString("00") + " : " + seconds.ToString("00");

        int gradePoints = 0;

        SetPoints(gradePoints, playerDeathAmount, playerDeathsVeryBad, playerDeathsBad, playerDeathsGood, playerDeathsVeryGood);
        SetPoints(gradePoints, enemiesAlerted, monsterAlertVeryBad, monsterAlertBad, monsterAlertGood, monsterAlertVeryGood);
        SetPoints(gradePoints, enemiesAggressive, monsterAggressiveVeryBad, monsterAggressiveBad, monsterAggressiveGood, monsterAggressiveVeryGood);
        SetPoints(gradePoints, (int)completionTime, timeVeryBad, timeBad, timeGood, timeVeryBad);        

        Debug.Log("Grade Points: " + gradePoints.ToString());

        ShowGradeReward(gradePoints);
        SaveSystem.instance.SaveCurrentState();
    }

    private void SetPoints(int gradePoints, int valueToCheck, int veryBadValue, int badValue, int goodValue, int veryGoodValue)
    {
        if (valueToCheck >= veryBadValue)
            gradePoints += veryBadPoints;
        else if (valueToCheck >= badValue)
            gradePoints += badPoints;
        else if (valueToCheck >= goodValue)
            gradePoints += goodPoints;
        else
            gradePoints += veryGoodPoints;
    }

    private void ShowGradeReward(int gradePoints)
    {
        if(gradePoints >= highestGrade)
        {
            highestGradeRewardObject.SetActive(true);
        }
        else if(gradePoints >= mediumGrade)
        {
            mediumGradeRewardObject.SetActive(true);
        }
        else
        {
            lowestGradeRewardObject.SetActive(true);
        }
    }

}
