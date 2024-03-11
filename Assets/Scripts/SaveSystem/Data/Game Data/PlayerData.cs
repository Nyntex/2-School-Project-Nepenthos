using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public Vector3 position;
    public Vector3 rotation;

    public PlayerData()
    {
        position = Vector3.zero;
        rotation = Vector3.zero;
    }

    public PlayerData(Vector3 position, Vector3 rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
