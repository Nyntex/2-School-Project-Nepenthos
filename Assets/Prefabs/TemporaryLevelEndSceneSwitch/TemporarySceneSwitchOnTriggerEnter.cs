using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class TemporarySceneSwitchOnTriggerEnter : MonoBehaviour
{
    public int sceneIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioController.Instance.CleanUp();
            SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
        }
    }
}
