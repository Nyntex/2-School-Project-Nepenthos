
[System.Serializable]
public class ActivatableObjectData
{
    public bool hasBeenActivated;

    public ActivatableObjectData()
    {
        hasBeenActivated = false;
    }

    public ActivatableObjectData(bool hasBeenActivated)
    {
        this.hasBeenActivated = hasBeenActivated;
    }
}
