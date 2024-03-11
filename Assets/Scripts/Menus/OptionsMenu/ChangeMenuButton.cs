using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMenuButton : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> menus;

    [SerializeField]
    private GameObject menuToActivate;

    public void OnClick()
    {
        foreach (GameObject menu in menus) 
        { 
            menu.SetActive(false);
        }

        menuToActivate.SetActive(true);
    }
}
