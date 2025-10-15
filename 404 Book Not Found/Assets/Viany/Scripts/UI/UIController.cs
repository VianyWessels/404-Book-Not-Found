using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas characterSelect;
    [SerializeField] private Canvas levelSelect;
    [SerializeField] private Canvas pauzeMenu;
    [SerializeField] private Canvas inGameCanvas;
    [SerializeField] private Canvas winScreen;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject characterSelectedPanel;

    [SerializeField] private InputActionReference resetPrefsAction;

    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Button[] levelButtons;

    private bool openedFromMainMenu;
    private bool openedFromPause;
    private bool characterChosen;

    void Start()
    {
        TimeScale(0);

        mainMenu.enabled = true;
        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;

        characterChosen = PlayerPrefs.GetInt("CharacterChosen", 0) == 1;

        if (characterSelectedPanel != null)
        {
            characterSelectedPanel.SetActive(true);
        }

        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;

        musicSlider.value = musicValue;
        sfxSlider.value = sfxValue;
        fullscreenToggle.isOn = fullscreen;

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);
        SetFullscreen(fullscreen);

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        UpdateLevelButtons();
    }

    private void OnEnable()
    {
        if (resetPrefsAction != null)
        {
            resetPrefsAction.action.performed += OnResetPrefs;
            resetPrefsAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (resetPrefsAction != null)
        {
            resetPrefsAction.action.performed -= OnResetPrefs;
            resetPrefsAction.action.Disable();
        }
    }

    private void OnResetPrefs(InputAction.CallbackContext context)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        characterChosen = false;
        mainMenu.enabled = true;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;

        if (characterSelectedPanel != null)
        {
            characterSelectedPanel.SetActive(false);
        }

        if (levelSystem != null)
        {
            levelSystem.ResetLevels();
        }

        UpdateLevelButtons();
    }

    public void OnStartButton()
    {
        TimeScale(0);

        mainMenu.enabled = false;

        if (!characterChosen)
        {
            openedFromMainMenu = true;
            ShowCharacterSelect();
        }
        else
        {
            ShowLevelSelect();
        }
    }

    private void ShowCharacterSelect()
    {
        TimeScale(0);

        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        characterSelect.enabled = true;
        mainMenu.enabled = false;
        settings.enabled = false;
        levelSelect.enabled = false;
    }

    public void OnCharacterConfirmed()
    {
        characterChosen = true;
        PlayerPrefs.SetInt("CharacterChosen", 1);
        PlayerPrefs.Save();
        characterSelect.enabled = false;

        if (characterSelectedPanel != null)
        {
            characterSelectedPanel.SetActive(true);
        }

        if (openedFromMainMenu)
        {
            ShowLevelSelect();
        }
        else
        {
            GoToMainMenu();
        }
    }

    public void OpenCharacterSelect()
    {
        TimeScale(0);

        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        openedFromMainMenu = false;
        characterSelect.enabled = true;
        mainMenu.enabled = false;
        settings.enabled = false;
        levelSelect.enabled = false;
    }

    private void ShowLevelSelect()
    {
        TimeScale(0);

        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        levelSelect.enabled = true;
        mainMenu.enabled = false;
        characterSelect.enabled = false;
        settings.enabled = false;

        UpdateLevelButtons();
    }

    private void UpdateLevelButtons()
    {
        if (levelButtons == null || levelSystem == null) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].interactable = levelSystem.IsLevelUnlocked(levelIndex);
            int capturedIndex = levelIndex;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => OnLevelButtonClicked(capturedIndex));
        }
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        levelSystem.LoadLevel(levelIndex);
        UpdateLevelButtons();
        levelSelect.enabled = false;
        StartGame();
    }

    public void StartGame()
    {
        TimeScale(1);

        inGameCanvas.enabled = true;
        pauzeMenu.enabled = false;
        mainMenu.enabled = false;
        settings.enabled = false;
    }

    public void GoToMainMenu()
    {
        TimeScale(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        settings.enabled = false;

        if (openedFromPause)
        {
            TimeScale(0);
            pauzeMenu.enabled = true;
        }
        else
        {
            mainMenu.enabled = true;
        }

        inGameCanvas.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    private void SetSFXVolume(float value)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void OpenSettingsFromPause()
    {
        TimeScale(0);

        openedFromPause = true;
        pauzeMenu.enabled = false;
        settings.enabled = true;
    }

    public void OpenSettingsFromMainMenu()
    {
        TimeScale(0);

        openedFromPause = false;
        mainMenu.enabled = false;
        settings.enabled = true;
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settings.enabled)
            {
                TimeScale(0);
                if (openedFromPause)
                {
                    settings.enabled = false;
                    pauzeMenu.enabled = true;
                }
                else
                {
                    settings.enabled = false;
                    mainMenu.enabled = true;
                }
            }
            else if (AllMenusOff())
            {
                TimeScale(0);
                pauzeMenu.enabled = true;
            }
        }
    }

    public void ReturnFromSettings()
    {
        TimeScale(0);
        if (openedFromPause)
        {
            settings.enabled = false;
            pauzeMenu.enabled = true;
        }
        else
        {
            settings.enabled = false;
            mainMenu.enabled = true;
        }
    }


    private bool AllMenusOff()
    {
        return !mainMenu.enabled && !settings.enabled && !characterSelect.enabled && !levelSelect.enabled;
    }

    public void GoToMainMenuFromPause()
    {
        TimeScale(0);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        pauzeMenu.enabled = false;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
        mainMenu.enabled = true;
        openedFromPause = false;
    }

    public void TimeScale(int scale)
    {
        Time.timeScale = scale;
    }

    public void RedoLevel()
    {
        Time.timeScale = 1f;
        var currentLevel = FindFirstObjectByType<LevelSystem>().GetCurrentLevel();
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowWinScreen()
    {
        TimeScale(0);

        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        winScreen.enabled = true;
    }

    public void OnWinMainMenuButton()
    {
        int currentLevel = levelSystem.GetCurrentLevel();
        levelSystem.UnlockNextLevel(currentLevel);
        winScreen.enabled = false;

        GoToMainMenu();
    }

    public void OnWinNextLevelButton()
    {
        if (levelSystem == null) return;

        int currentLevel = levelSystem.GetCurrentLevel();
        int nextLevel = currentLevel + 1;

        levelSystem.UnlockNextLevel(currentLevel);

        winScreen.enabled = false;

        if (nextLevel <= levelSystem.GetTotalLevels())
        {
            levelSystem.LoadLevel(nextLevel);
            StartGame();
        }
        else
        {
            GoToMainMenu();
        }
    }
}