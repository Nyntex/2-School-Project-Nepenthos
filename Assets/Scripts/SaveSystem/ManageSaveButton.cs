using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManageSaveButton : MonoBehaviour
{
    public void LoadSave(int saveGame)
    {
        SaveSystem.instance.ChooseSaveGame(saveGame);
        Debug.Log(SaveSystem.instance.currentSave);
    }

    public void DeleteSave(int saveGame)
    {
        SaveSystem.instance.DeleteSaveGame(saveGame);
        Debug.Log("Deleting Save File");
    }
}
