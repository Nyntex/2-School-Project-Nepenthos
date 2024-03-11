using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class StaminaBar : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if(player == null)
            player = FindFirstObjectByType<PlayerController>(FindObjectsInactive.Include);
    }

    void Update()
    {
        slider.maxValue = player.MaxStamina;
        slider.value = player.currentStamina;
    }
}
