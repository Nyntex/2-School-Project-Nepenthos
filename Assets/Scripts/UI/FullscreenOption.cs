using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenOption : MonoBehaviour, IConfigSaveable
{
    [SerializeField]
    private Toggle fullscreenToggle;

    private bool isFullscreen;

    private void OnEnable()
    {
        fullscreenToggle.isOn = isFullscreen;
        ChangeScreenType(isFullscreen);
    }

    public void ChangeScreenType(bool isFullscreen)
    {
        this.isFullscreen = isFullscreen;
        SaveSystem.instance.ChangeScreenType(isFullscreen);
    }

    public void LoadData(ConfigData data)
    {
        isFullscreen = data.isFullscreen;
        fullscreenToggle.isOn = isFullscreen;
        ChangeScreenType(isFullscreen);
    }

    public void SaveData(ref ConfigData data)
    {
        data.isFullscreen = isFullscreen;
    }
}
