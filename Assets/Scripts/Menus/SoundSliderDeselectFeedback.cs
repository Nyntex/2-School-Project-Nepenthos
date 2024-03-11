using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FMODUnity;

[RequireComponent(typeof(Slider))]
public class SoundSliderDeselectFeedback : MonoBehaviour, IPointerUpHandler
{
    [SerializeField]
    private EventReference soundToPlay;

    public void OnPointerUp(PointerEventData eventData)
    {
        if (soundToPlay.IsNull) return;
        AudioController.Instance.PlayOneShot(soundToPlay, FindFirstObjectByType<Camera>().transform.position);
    }
}
