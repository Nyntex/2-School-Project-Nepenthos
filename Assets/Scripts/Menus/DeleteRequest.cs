using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRequest : MonoBehaviour
{
    [SerializeField]
    private GameObject prompt;
    private int requestedDeletion = 0;

    public void OnClickYes()
    {
        SaveSystem.instance.DeleteSaveGame(requestedDeletion);
        DisablePrompt();
    }

    public void OnClickNo()
    {
        DisablePrompt();
    }

    public void Request(int saveID)
    {
        prompt.SetActive(true);
        requestedDeletion = saveID;
    }

    public void DisablePrompt()
    {
        prompt.SetActive(false);
    }
}
