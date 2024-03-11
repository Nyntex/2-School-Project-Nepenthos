using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingMenuCamera : MonoBehaviour
{
    [SerializeField]
    private float lerpSpeed = 0.2f;
    [SerializeField]
    private GameObject firstObjectToMoveTo;

    private Vector3 positionToMoveTo;

    public float currentSpeed;
    //private Vector3 originalPosition;

    private void Awake()
    {
        //originalPosition = transform.position;
        positionToMoveTo = transform.position;
    }

    private void Start()
    {
        StartCoroutine(DelayedFirstMove(0.5f));
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        currentSpeed = lerpSpeed * Time.unscaledDeltaTime;
        transform.position = Vector3.Lerp(transform.position, positionToMoveTo, lerpSpeed * Time.unscaledDeltaTime);
    }

    public void MoveCamera(Vector3 position)
    {
        positionToMoveTo = position;
    }

    public IEnumerator DelayedFirstMove(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        positionToMoveTo = firstObjectToMoveTo.transform.position;
    }
}
