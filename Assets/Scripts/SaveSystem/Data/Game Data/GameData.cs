[System.Serializable]
public class GameData
{
    public SerializeableDictionary<int,PlayerData> playerData;
    public SerializeableDictionary<string, EnemyData> enemyDataDictionary;
    public SerializeableDictionary<string, DistractionObjectData> distractionObjectDataDictionary;
    public SerializeableDictionary<string, ActivatableObjectData> activatableObjectDataDictionary;
    public TrackingData trackingData;

    public GameData() 
    { 
        playerData = new SerializeableDictionary<int, PlayerData>();
        enemyDataDictionary = new SerializeableDictionary<string, EnemyData>();
        distractionObjectDataDictionary = new SerializeableDictionary<string, DistractionObjectData>();
        activatableObjectDataDictionary = new SerializeableDictionary<string, ActivatableObjectData>();
        trackingData = new TrackingData();
    }

}
