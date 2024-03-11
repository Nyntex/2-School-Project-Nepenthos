using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    [Header("Save System Configuration")]
    private string saveName1 = "save1.save";
    public string SaveName1 { get { return saveName1;} }
    private string saveName2 = "save2.save";
    public string SaveName2 { get { return saveName2; } }
    private string saveName3 = "save3.save";
    public string SaveName3 { get { return saveName3; } }

    public string currentSave { get; private set; }

    [SerializeField]
    private string configName;

    [Space(5)]
    [SerializeField]
    private bool enableSaving = true;

    private GameData gameData;
    private ConfigData configData;

    private List<ISaveable> saveableObjects;
    private List<IConfigSaveable> saveableConfigs;

    private FileDataHandler fileDataHandler;

    public static SaveSystem instance { get; private set; }

    private void Awake()
    {
        if (instance != null ) 
        {
            Debug.LogError("Found more than one SaveSystem in the Scene");
            Destroy(gameObject);
            return;
        }
        instance = this;
        //currentSave = SaveName1;
        DontDestroyOnLoad(instance);
    }

    private void Start()
    {
        if (!enableSaving) { return; }

        fileDataHandler = new FileDataHandler(Application.persistentDataPath + "/saves", Application.persistentDataPath, configName);
        instance.saveableObjects = FindAllSaveableObjects();
        instance.saveableConfigs = FindAllSaveableConfigs();
        instance.Load();
        SetLastSaveCurrentSave();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void NewGame()
    {
        instance.gameData = new GameData();
        instance.SaveGame();
    }

    public void NewConfig()
    {
        instance.configData = new ConfigData();
        instance.SaveConfig();
    }

    public void Load()
    {
        if (!instance.enableSaving) { return; }
        instance.LoadGame();
        instance.LoadConfig();
    }

    public void LoadGame()
    {
        if (instance.currentSave == null) return;
        // Load data from file through data handler
        instance.gameData = fileDataHandler.LoadSave(instance.currentSave);

        // If no Data found, initialize to new game
        if(instance.gameData == null)
        {
            Debug.Log("No save data was loaded. Initiliazing save data to defaults.");
            instance.NewGame();
        }

        for (int i = instance.saveableObjects.Count - 1; i >= 0; i--)
        {
            if (instance.saveableObjects[i].Equals(null))
            {
                instance.saveableObjects.RemoveAt(i);
            }
        }

        for (int i= 0; i < instance.saveableObjects.Count; i++) 
        {
            instance.saveableObjects[i].LoadData(instance.gameData);
            //Debug.Log(instance.saveableObjects[i]);
        }

        /*
        //Push data to objects
        foreach (ISaveable saveableObject in instance.saveableObjects)
        {
            //if(saveableObject == null)
            //{
            //    Debug.Log("Found null object: " + saveableObject.ToString());
            //    continue;
            //}
            saveableObject.LoadData(instance.gameData);
            Debug.Log(saveableObject.ToString());
        }

        foreach (ISaveable saveableObject in instance.saveableObjects)
        {
            Debug.Log(saveableObject);
        }
        */
        //Debug.Log(gameData.playerData.ToString());
        Debug.Log("Loading Game Complete");
        AudioController.Instance.Pause(false);
    }

    public void LoadConfig()
    {
        instance.configData = fileDataHandler.LoadConfig();

        if (instance.configData == null)
        {
            Debug.Log("No config data was loaded. Initiliazing config data to defaults.");
            instance.NewConfig();
        }

        for (int i = instance.saveableConfigs.Count - 1; i >= 0; i--)
        {
            if (instance.saveableConfigs[i].Equals(null))
            {
                instance.saveableConfigs.RemoveAt(i);
            }
        }

        foreach (IConfigSaveable saveableConfig in instance.saveableConfigs)
        {
            if (saveableConfig.Equals(null)) continue;
            //Debug.Log(saveableConfig.ToString());
            saveableConfig.LoadData(instance.configData);
        }

        ChangeScreenType(configData.isFullscreen);

        Debug.Log("Loading Config Complete");
    }

    public void Save()
    {
        if (!instance.enableSaving) { return; }
        Debug.Log("Saving File: " + instance.currentSave);
        instance.SaveGame();
        instance.SaveConfig();
        Debug.Log("Saving Complete");
    }

    public void SaveGame()
    {
        if (instance.currentSave == null) currentSave = SaveName1;
        if (instance.saveableObjects.Count < 1)
        {
            Debug.LogError("There are no Objects in the scene to save.");
            return;
        }
        //remove null objects
        for (int i = instance.saveableObjects.Count - 1; i >= 0; i--)
        {
            if (instance.saveableObjects[i].Equals(null))
            {
                instance.saveableObjects.RemoveAt(i);
            }
        }
        //get all the Data from all scripts that save
        foreach (ISaveable saveableObject in instance.saveableObjects)
        {
            if(!saveableObject.Equals(null))
            {
                saveableObject.SaveData(ref instance.gameData);
            }
        }

        //save the data to a file
        fileDataHandler.SaveGame(instance.gameData, instance.currentSave);
    }

    public void SaveConfig()
    {
        if (instance.saveableConfigs.Count < 1) return;
        //remove null objects
        for (int i = instance.saveableConfigs.Count - 1; i >= 0; i--)
        {
            if (instance.saveableConfigs[i].Equals(null))
            {
                instance.saveableConfigs.RemoveAt(i);
            }
        }

        foreach (IConfigSaveable saveableConfig in instance.saveableConfigs)
        {
            Debug.Log(saveableConfig);
            if(!saveableConfig.Equals(null))
            {
                saveableConfig.SaveData(ref instance.configData);
            }
        }

        fileDataHandler.SaveConfig(configData);
    }
    

    private List<ISaveable> FindAllSaveableObjects()
    {
        IEnumerable<ISaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>();

        return new List<ISaveable>(saveableObjects);
    }

    private List<IConfigSaveable> FindAllSaveableConfigs()
    {
        IEnumerable<IConfigSaveable> saveableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IConfigSaveable>();

        return new List<IConfigSaveable>(saveableObjects);
    }

    private void OnApplicationQuit()
    {
        instance.SaveConfig();
    }

    public void OnSceneLoaded(Scene ignore, LoadSceneMode me)
    {
        instance.saveableConfigs = FindAllSaveableConfigs();
        instance.saveableObjects = FindAllSaveableObjects();
        instance.Load();
    }

    public void ChooseSaveGame(int number)
    {
        switch (number)
        {
            case 2:
                instance.currentSave = instance.saveName2;
                instance.configData.lastSave = 2;
                break;
            case 3:
                instance.currentSave = instance.saveName3;
                instance.configData.lastSave = 3;
                break;
            case 1:
            default:
                instance.currentSave = instance.saveName1;
                instance.configData.lastSave = 1;
                break;
        }
        instance.LoadGame();
    }

    public void DeleteSaveGame(int number)
    {
        switch (number)
        {
            case 2:
                instance.currentSave = instance.saveName2;
                break;
            case 3:
                instance.currentSave = instance.saveName3;
                break;
            case 1:
            default:
                instance.currentSave = instance.saveName1;
                break;
        }
        Debug.Log("Deleting: " + instance.currentSave);
        fileDataHandler.DeleteSaveGame(instance.currentSave);
        //instance.currentSave = "";
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex > 3) 
        {
            instance.gameData.trackingData.completionTime += Time.deltaTime;
            //Debug.Log(instance.gameData);
            //Debug.Log(instance.gameData.trackingData);
            //Debug.Log(instance.gameData.trackingData.completionTime);
        }
    }

    public void IncreasePlayerDeath(int amount = 1)
    {
        instance.gameData.trackingData.playerDeaths += amount;
    }

    public ref TrackingData GetTrackingData()
    {
        return ref instance.gameData.trackingData;
    }

    public void SaveCurrentState()
    {
        fileDataHandler.SaveGame(instance.gameData, instance.currentSave);
    }

    public void ActivateObject(string id)
    {
        if(instance.gameData.activatableObjectDataDictionary.ContainsKey(id))
        {
            instance.gameData.activatableObjectDataDictionary[id].hasBeenActivated = true;
        }
        else
        {
            instance.gameData.activatableObjectDataDictionary.Add(id, new ActivatableObjectData(true));
        }
        SaveCurrentState();
    }

    public void SetLastSaveCurrentSave()
    {
        Debug.Log("CurrentSave Before: " +instance.currentSave);
        Debug.Log("LastSave Before: " + instance.configData.lastSave);
        if (instance.configData.lastSave > 0 && instance.configData.lastSave < 4)
        {
            ChooseSaveGame(instance.configData.lastSave);
        }
        else
        {
            instance.currentSave = instance.saveName1;
            instance.configData.lastSave = 1;
        }
        instance.SaveConfig();
        Debug.Log("CurrentSave After: " + instance.currentSave);
        Debug.Log("LastSave After: " + instance.configData.lastSave);
    }

    public void ChangeScreenType(bool isFullscreen)
    {
        if (isFullscreen)
        {
            Screen.fullScreen = true;
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else
        {
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
        Debug.Log(Screen.fullScreenMode);
        Debug.Log(Screen.fullScreen);
        SaveSystem.instance.SaveConfig();
    }
}
