using System.Collections.Generic;
using UnityEngine;



public class DeleteSavesButton : MonoBehaviour
{
    [SerializeField]
    private GameObject saveButtons;

    [SerializeField]
    private GameObject deleteButtons;

    public void SwitchButtons()
    {
        saveButtons.SetActive(!saveButtons.activeSelf);
        deleteButtons.SetActive(!saveButtons.activeSelf);
    }
}
