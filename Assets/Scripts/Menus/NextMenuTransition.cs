using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NextMenuTransition : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraToActivateTransition;

    public void OnClick()
    {
        cameraToActivateTransition.SetActive(!cameraToActivateTransition.activeSelf);
        
    }
}
