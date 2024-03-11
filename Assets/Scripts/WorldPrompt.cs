using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPrompt : MonoBehaviour
{
    private Camera playerCamera;
    
    void Awake()
    {
        var cameras = FindObjectsOfType<Camera>();
        foreach (var cam in cameras) 
        {
            if (cam.CompareTag("MainCamera"))
            {
                playerCamera = cam;
            }
        }
    }

    void Update()
    {
        transform.LookAt(playerCamera.transform.position);        
    }
}
