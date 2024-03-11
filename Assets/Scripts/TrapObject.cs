using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : MonoBehaviour
{
    [SerializeField][Min(0.1f)]
    private float trapDuration = 2f;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<ITrappable>() != null)
        {
            StartCoroutine(Trap(other.GetComponent<ITrappable>()));
        }
    }

    IEnumerator Trap(ITrappable target)
    {
        target.GetTrapped();
        yield return new WaitForSeconds(trapDuration);
        target.GetReleased();
    }
}