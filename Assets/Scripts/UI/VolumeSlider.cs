using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour, IPointerUpHandler//, IConfigSaveable
{
    [SerializeField] 
    private VolumeType volumeType;
    [SerializeField]
    private Slider volumeSlider;

    private void Start()
    {
        if (volumeSlider == null)
        {
            volumeSlider = GetComponentInChildren<Slider>(true);
        }
        if (volumeSlider == null )
        {
            volumeSlider = GetComponent<Slider>();
        }
        if ( volumeSlider == null )
        {
            Debug.LogError("Volume Slider Script of '" + gameObject.name + "' has no Slider on itself or its children!");
        }
        ReloadSlider();
    }

    private void OnEnable()
    {
        ReloadSlider();
    }

    //private void Update()
    //{
    //    OnSliderValueChanged();
    //}

    //public void OnSliderValueChanged()
    //{
    //    switch (volumeType)
    //    {
    //        case VolumeType.MASTER:
    //            AudioController.Instance.masterVolume = volumeSlider.value;
    //            break;
    //        case VolumeType.MUSIC:
    //            AudioController.Instance.musicVolume = volumeSlider.value;
    //            break;
    //        case VolumeType.AMBIENCE:
    //            AudioController.Instance.ambienceVolume = volumeSlider.value;
    //            break;
    //        case VolumeType.SFX:
    //            AudioController.Instance.SFXVolume = volumeSlider.value;
    //            break;
    //        default:
    //            Debug.LogWarning("Volumetype not supported: " + volumeType);
    //            break;
    //    }

    //    AudioController.Instance.UpdateVolume();
    //}

    public void OnPointerUp(PointerEventData eventData)
    {

        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioController.Instance.masterVolume = volumeSlider.value;
                break;
            case VolumeType.MUSIC:
                AudioController.Instance.musicVolume = volumeSlider.value;
                break;
            case VolumeType.AMBIENCE:
                AudioController.Instance.ambienceVolume = volumeSlider.value;
                break;
            case VolumeType.SFX:
                AudioController.Instance.SFXVolume = volumeSlider.value;
                break;
            default:
                Debug.LogWarning("Volumetype not supported: " + volumeType);
                break;
        }

        AudioController.Instance.UpdateVolume();
        SaveSystem.instance.SaveConfig();
    }

    public void ReloadSlider()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioController.Instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioController.Instance.musicVolume;
                break;
            case VolumeType.AMBIENCE:
                volumeSlider.value = AudioController.Instance.ambienceVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioController.Instance.SFXVolume;
                break;
            default:
                Debug.LogWarning("Volumetype not supported: " + volumeType);
                break;
        }
    }
}
