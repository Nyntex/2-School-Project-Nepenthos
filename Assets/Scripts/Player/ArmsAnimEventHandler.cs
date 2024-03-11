using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsAnimEventHandler : MonoBehaviour
{
    private PlayerController playerController;
    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    public void OnGrab()
    {
        playerController.OnGrab();
    }
    public void OnThrow()
    {
        playerController.OnThrow();
    }
}