using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider))]
public class WinArea : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UIController uiController;

    [SerializeField]
    private Animation anim;

    void Awake()
    {
        if(uiController == null)
            uiController = FindObjectOfType<UIController>(true);
        if(canvas == null)
            canvas = GetComponentInChildren<Canvas>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            AudioController.Instance.Pause(true);
            uiController.gameObject.SetActive(false);
            anim.Play("FadeToBlack");
        }
    }

    public void OnContinue()
    {
        uiController.gameObject.SetActive(true);
        canvas.gameObject.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void OnRetry()
    {
        AudioController.Instance.CleanUp();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void OnQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        uiController.BackgroundBlur(false);
#else
        Application.Quit();
#endif
    }

    public void OnFadeOutFinished()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }
}
