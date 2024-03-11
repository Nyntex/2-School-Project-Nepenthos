using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void OnHover()
    {
        AudioController.Instance.PlayOneShot(AudioController.Instance.buttonHover_ref, transform.position);
    }
    public void OnClick()
    {
        AudioController.Instance.PlayOneShot(AudioController.Instance.buttonClick_ref, transform.position);
    }
}
