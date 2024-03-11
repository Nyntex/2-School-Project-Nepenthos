using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceVolume : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private string parameterName;
    [SerializeField] private float parameterMaxValue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            AudioController.Instance.SetAmbienceParameter(parameterName, parameterMaxValue);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            AudioController.Instance.SetAmbienceParameter(parameterName, 0f);
        }
    }
}
