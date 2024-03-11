using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField]
    private GameObject killPrompt;
    [SerializeField]
    private GameObject deathScreen;
    [SerializeField]
    private GameObject pauseScreen;
    [SerializeField]
    private GameObject killcount;
    [SerializeField]
    private GameObject optionsScreen;
    [SerializeField]
    private GameObject staminaBar;
    [SerializeField]
    private GameObject crosshair;
    [SerializeField]
    private GameObject sneakIcon;

    [Space(5)]
    [SerializeField]
    private Animator fadeToMainMenu;
    [SerializeField]
    private int sceneToLoadFromPauseMenu;

    private Controls inputActions;

    public ScriptableRendererFeature backgroundBlur;

    public Action<bool> gamePause;

    private bool killPromptActive = false;
    private bool crosshairActive = false;
    private bool sneakIconActive = false;

    private void Awake()
    {
        inputActions = new Controls();
        ClearPopupScreens();
    }
    private void OnEnable()
    {
        inputActions.Enable();
    }
    private void OnDisable()
    {
        inputActions.Disable();
    }
    private void Update()
    {
        if (deathScreen.gameObject.activeSelf) return;
        if (inputActions.UI.Pause.WasPerformedThisFrame())
        {
            if (optionsScreen.activeSelf)
            {
                BackFromOptions();
            }
            else
            {
                GamePause();
            }
        }
    }
    private void ClearPopupScreens()
    {
        killPrompt.SetActive(false);
        killPromptActive = false;
        ShowScreen(deathScreen, false);
        ShowScreen(pauseScreen, false);
        ShowScreen(optionsScreen, false);
        BackgroundBlur(pauseScreen.gameObject.activeSelf);
    }
    public void ShowKillPrompt(bool val)
    {
        killPrompt.SetActive(val);
        killPromptActive = val;
    }
    public void ShowSneakIcon(bool val)
    {
        sneakIcon.SetActive(val);
        sneakIconActive = val;
    }
    private void ShowScreen(GameObject screen, bool val)
    {
        screen.SetActive(val);
        if (val)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }
    public void ShowDeathScreen(bool val)
    {
        ShowScreen(deathScreen, val);
        FindObjectOfType<RenderFeatureToggler>().DeactivateOptionalRenderFeatures();
    }
    public void ShowCrosshair(bool val)
    {
        crosshair.SetActive(val);
        crosshairActive = val;
    }
    public void BackgroundBlur(bool val)
    {
        backgroundBlur.SetActive(val);
    }
    private void GamePause()
    {
        bool pauseState = pauseScreen.gameObject.activeSelf;
        ShowScreen(pauseScreen, !pauseState);
        BackgroundBlur(!pauseState);
        killcount.gameObject.SetActive(pauseState);
        if (killPromptActive)
        {
            killPrompt.SetActive(pauseState);
        }
        staminaBar.gameObject.SetActive(pauseState);
        if(crosshairActive)
        {
            crosshair.SetActive(pauseState);
        }
        if(sneakIconActive)
        {
            sneakIcon.SetActive(pauseState);
        }

        AudioController.Instance.Pause(!pauseState);
        gamePause.Invoke(pauseState);
    }
    public void OnRetry()
    {
        FindObjectOfType<RenderFeatureToggler>().DeactivateOptionalRenderFeatures();
        AudioController.Instance.CleanUp();
        SaveSystem.instance.SaveConfig();
        //Destroy(gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex,LoadSceneMode.Single);
        //SaveSystem.instance.Load();
        //ClearPopupScreens();
        //killcount.SetActive(true);
        //staminaBar.SetActive(true);
    }

    public void OnContinue()
    {
        ClearPopupScreens();
        killcount.gameObject.SetActive(true);
        staminaBar.gameObject.SetActive(true);
        AudioController.Instance.Pause(pauseScreen.gameObject.activeSelf);
    }

    public void OnOptions()
    {
        ShowScreen(pauseScreen, false);
        ShowScreen(optionsScreen, true);
        //BackgroundBlur(true);
    }

    public void BackFromOptions()
    {
        ShowScreen(optionsScreen, false);
        ShowScreen(pauseScreen, true);
        SaveSystem.instance.SaveConfig();
    }

    public void OnMainMenuClick()
    {
        if(fadeToMainMenu == null)
        {
            FindObjectOfType<RenderFeatureToggler>().DeactivateOptionalRenderFeatures();
            AudioController.Instance.CleanUp();
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(sceneToLoadFromPauseMenu);
            return;
        }
        fadeToMainMenu.SetTrigger("Start");
    }

    public void FadeToMainMenuFinish()
    {
        FindObjectOfType<RenderFeatureToggler>().DeactivateOptionalRenderFeatures();
        AudioController.Instance.CleanUp();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneToLoadFromPauseMenu);
    }


    public void OnQuit()
    {
        FindObjectOfType<RenderFeatureToggler>().DeactivateOptionalRenderFeatures();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        BackgroundBlur(false);
#else
        Application.Quit();
#endif
    }


}