using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingMenuCamera_Repositioner : MonoBehaviour
{
    [SerializeField]
    private MovingMenuCamera cam;

    [SerializeField]
    private GameObject moveToPosition;


    public void MoveCamera()
    {
        cam.MoveCamera(moveToPosition.transform.position);
    }
}
