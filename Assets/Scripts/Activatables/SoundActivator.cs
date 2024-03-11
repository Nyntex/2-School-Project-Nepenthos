using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundActivator : ActivatableObject
{
    [Space(20)]
    [SerializeField]
    private StudioEventEmitter soundToPlay;

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasBeenActivated) return;
        soundToPlay.Play();
        GetComponent<BoxCollider>().enabled = false;
        hasBeenActivated = true;
        SaveSystem.instance.ActivateObject(id);
    }
}
