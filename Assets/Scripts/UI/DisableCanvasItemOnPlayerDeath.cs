using UnityEngine;

public class DisableCanvasItemOnPlayerDeath : MonoBehaviour
{
    private PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerController.PlayerDied += OnPlayerDeath;
    }

    public void OnPlayerDeath()
    {
        gameObject.SetActive(false);
    }

}
