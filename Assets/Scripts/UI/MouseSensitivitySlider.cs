using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class MouseSensitivitySlider : MonoBehaviour, IPointerUpHandler, IConfigSaveable
{
    private CameraController cam;
    private Slider slider;

    private void Start()
    {
        cam = FindFirstObjectByType<CameraController>();
        slider = GetComponent<Slider>();
        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            slider = GetComponent<Slider>();
            gameObject.SetActive(false);
        }
        if(cam != null )
        {
            slider.value = cam.cameraSensitivity.x;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {

        if (cam != null)
        {
            cam.cameraSensitivity = new Vector2(slider.value, slider.value);
        }
        SaveSystem.instance.SaveConfig();
    }

    public void LoadData(ConfigData data)
    {
        Debug.Log(slider.ToString());
        Debug.Log(data.mouseSensitivity.ToString());
        Debug.Log(cam.ToString());
        if(slider = null)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                slider = GetComponent<Slider>();
                slider.value = data.mouseSensitivity;
                gameObject.SetActive(false);
            }
            else
            {
                slider = GetComponent<Slider>();
                slider.value = data.mouseSensitivity;
            }
        }
        else 
        {
            slider.value = data.mouseSensitivity;
        }
    }

    public void SaveData(ref ConfigData data)
    {
        if (slider = null) return;
        data.mouseSensitivity = slider.value;
    }
}
