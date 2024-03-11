using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioData
{
    public float masterVolume;
    public float musicVolume;
    public float effectVolume;
    public float ambienceVolume;

    public AudioData(float masterVolume, float musicVolume, float effectVolume, float ambienceVolume)
    {
        this.masterVolume = masterVolume;
        this.musicVolume = musicVolume;
        this.effectVolume = effectVolume;
        this.ambienceVolume = ambienceVolume;
    }

    public AudioData() 
    {
        masterVolume = 0.2f;
        musicVolume = 0.2f;
        effectVolume = 0.2f;
        ambienceVolume = 0.2f;
    }
}
