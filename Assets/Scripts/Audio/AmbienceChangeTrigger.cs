using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbienceChangeTrigger : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private string parameterName;
    [SerializeField] private float parameterValue;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3)
        {
            AudioController.Instance.SetAmbienceParameter(parameterName, parameterValue);
        }
    }
}
