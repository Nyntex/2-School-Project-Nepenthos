using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string saveDirectoryPath = "";
    private string configDirectoryPath = "";
    private string configName = "";

    public FileDataHandler(string savePath, string configPath, string configName)
    {
        this.saveDirectoryPath = savePath;
        this.configDirectoryPath = configPath;
        this.configName = configName;
    }

    public GameData LoadSave(string saveName)
    {
        string fullpath = Path.Combine(saveDirectoryPath, saveName);
        GameData loadData = null;
        if (File.Exists(fullpath)) 
        {
            try
            {
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream)) 
                    { 
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("An error occured whilst trying to load data from file: " + fullpath + "\n" + e);
            }
        }
        return loadData;
    }

    public ConfigData LoadConfig()
    {
        string fullpath = Path.Combine(configDirectoryPath, configName);
        ConfigData loadData = null;
        if (File.Exists(fullpath))
        {
            Debug.Log("File Exists: " + fullpath);
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullpath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadData = JsonUtility.FromJson<ConfigData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("An error occured whilst trying to load data from file: " + fullpath + "\n" + e);
            }
        }
        return loadData;
    }

    public void SaveGame(GameData data, string saveName)
    {
        string fullpath = Path.Combine(saveDirectoryPath, saveName);

        try 
        {
            //create the directory where the file will be stored if it doesn't exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            //serialize the GamData Object into json
            string dataToStore = JsonUtility.ToJson(data, true);

            //write the serialized data to teh file
            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream)) 
                { 
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e) 
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullpath + "\n" + e);
        }
    }

    public void SaveConfig(ConfigData data)
    {
        string fullpath = Path.Combine(configDirectoryPath, configName);

        try
        {
            //create the directory where the file will be stored if it doesn't exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullpath));

            //serialize the GamData Object into json
            string dataToStore = JsonUtility.ToJson(data, true);

            //write the serialized data to teh file
            using (FileStream stream = new FileStream(fullpath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

        }
        catch (Exception e)
        {
            Debug.LogError("Error occured when trying to save data to file: " + fullpath + "\n" + e);
        }
    }

    public void DeleteSaveGame(string saveName)
    {
        File.Delete(saveDirectoryPath + "/" + saveName);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

}
