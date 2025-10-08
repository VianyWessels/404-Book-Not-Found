using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas characterSelect;
    [SerializeField] private Canvas levelSelect;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject characterSelectedPanel;

    [SerializeField] private InputActionReference resetPrefsAction;

    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Button[] levelButtons;

    private bool openedFromMainMenu;
    private bool characterChosen;

    void Start()
    {
        mainMenu.enabled = true;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
        Time.timeScale = 0f;

        characterChosen = PlayerPrefs.GetInt("CharacterChosen", 0) == 1;

        if (characterChosen && characterSelectedPanel != null)
            characterSelectedPanel.SetActive(true);

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

        characterChosen = false;
        mainMenu.enabled = true;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
        Time.timeScale = 0f;

        if (characterSelectedPanel != null)
            characterSelectedPanel.SetActive(false);

        if (levelSystem != null)
            levelSystem.ResetLevels();

        UpdateLevelButtons();
    }

    public void OnStartButton()
    {
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
            characterSelectedPanel.SetActive(true);

        if (openedFromMainMenu)
            ShowLevelSelect();
        else
            GoToMainMenu();
    }

    public void OpenCharacterSelect()
    {
        openedFromMainMenu = false;
        characterSelect.enabled = true;
        mainMenu.enabled = false;
        settings.enabled = false;
        levelSelect.enabled = false;
    }

    private void ShowLevelSelect()
    {
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
        levelSystem.UnlockNextLevel(levelIndex);
        UpdateLevelButtons();
        levelSelect.enabled = false;
        StartGame();
    }

    public void StartGame()
    {
        mainMenu.enabled = false;
        settings.enabled = false;
        Time.timeScale = 1f;
    }

    public void GoToMainMenu()
    {
        mainMenu.enabled = true;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
        Time.timeScale = 0f;
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
}
