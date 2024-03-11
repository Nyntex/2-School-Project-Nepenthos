using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public Vector3 position;
    public Quaternion rotation;
    public int stateType;
    public float health;
    public int waypointIndex;
    public bool destroyed;
    public float alertnessSpeed;

    public EnemyData(Vector3 position, Quaternion rotation, int type, float health, int waypointIndex, bool destroyed, float alertnessSpeed)
    {
        this.position = position;
        this.rotation = rotation;
        this.stateType = type;
        this.health = health;
        this.waypointIndex = waypointIndex;
        this.destroyed = destroyed;
        this.alertnessSpeed = alertnessSpeed;
    }

    public EnemyData()
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        stateType = 0;
        health = 0;
        waypointIndex = 0;
        destroyed = false;
        alertnessSpeed = 0;
    }
}
