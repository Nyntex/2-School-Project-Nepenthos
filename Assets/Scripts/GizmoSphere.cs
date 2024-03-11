using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoSphere : MonoBehaviour
{
    [SerializeField]
    private float radius = 0.5f;

    [SerializeField]
    private Color color = Color.magenta;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position, radius);
    }
}
