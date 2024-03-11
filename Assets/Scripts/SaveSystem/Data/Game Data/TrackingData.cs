
[System.Serializable]
public class TrackingData
{
    public int enemiesAlerted;
    public int enemiesAggressive;
    public int playerDeaths;
    public float completionTime;

    public TrackingData()
    {
        enemiesAlerted = 0;
        enemiesAggressive = 0;
        playerDeaths = 0;
        completionTime = 0f;
    }
}
