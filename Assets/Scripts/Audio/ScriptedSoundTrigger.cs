using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptedSoundTrigger : MonoBehaviour
{
    [SerializeField]
    private EventReference soundEvent;
    [SerializeField]
    private GameObject soundPosition;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AudioController.Instance.PlayOneShot(soundEvent, soundPosition.transform.position);
            Destroy(gameObject);
        }
    }
}
