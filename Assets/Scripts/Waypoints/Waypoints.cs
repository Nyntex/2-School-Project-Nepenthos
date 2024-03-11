using Unity.VisualScripting;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [SerializeField]
    private float radius = 0.5f;

    [SerializeField]
    private Color color = Color.magenta;

    [SerializeField,Range(0.0f,1.0f)]
    private float chanceToWaitHere;
    public float ChanceToWaitHere { get { return chanceToWaitHere; } }

    [SerializeField]
    private float timeToWaitHere = 1.0f;
    public float TimeToWaitHere { get { return timeToWaitHere; } }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawSphere(transform.position,radius);
    }
}
