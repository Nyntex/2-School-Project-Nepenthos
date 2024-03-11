using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChangeTrigger : MonoBehaviour
{
    [Header("Music Area")]
    [SerializeField] private MusicArea area;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            AudioController.Instance.SetMusicArea(area);
        }
    }
}
