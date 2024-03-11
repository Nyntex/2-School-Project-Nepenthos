using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    private int sceneIndexToLoad;
    public void LoadSceneByIndex(int index)
    {
        if (GetComponent<UIController>() != null)
            GetComponent<UIController>().BackgroundBlur(false);

        AudioController.Instance.CleanUp();
        SaveSystem.instance.SaveConfig();
        sceneIndexToLoad = index;
        switch (sceneIndexToLoad)
        {
            case 0:
            case 1:
                AudioController.Instance.SetMusicArea(MusicArea.MAINMENU);
                break;
            default:
                AudioController.Instance.SetMusicArea(MusicArea.LEVEL_MUSIC);
                break;
        }
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneIndexToLoad,LoadSceneMode.Single);
        SaveSystem.instance.LoadConfig();
        //SaveSystem.instance.Save();
    }
}
