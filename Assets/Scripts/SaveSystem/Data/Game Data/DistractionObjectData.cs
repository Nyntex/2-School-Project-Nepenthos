using UnityEngine;

[System.Serializable]
public class DistractionObjectData
{
    public Vector3 position;
    public Quaternion rotation;
    public DistractionObject.DistractionObjectState state;

    public DistractionObjectData()
    { }

    public DistractionObjectData(Vector3 position, Quaternion rotation, DistractionObject.DistractionObjectState state)
    {
        this.position = position;
        this.rotation = rotation;
        this.state = state;
    }
}
