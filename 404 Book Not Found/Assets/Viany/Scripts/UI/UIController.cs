using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class CharacterImageData
{
    public Sprite winSprite;
    public Sprite deathSprite;
}

[System.Serializable]
public class CharacterTutorialData
{
    public Sprite[] tutorialSprites = new Sprite[2];
}

public class UIController : MonoBehaviour
{
    [SerializeField] private Canvas mainMenu;
    [SerializeField] private Canvas settings;
    [SerializeField] private Canvas characterSelect;
    [SerializeField] private Canvas levelSelect;
    [SerializeField] private Canvas pauzeMenu;
    [SerializeField] private Canvas inGameCanvas;
    [SerializeField] private Canvas winScreen;
    [SerializeField] private Canvas deathScreen;
    [SerializeField] private Canvas tutorialCanvas;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private Button leftArrowButton;
    [SerializeField] private Button rightArrowButton;
    [SerializeField] private Button exitTutorialButton;
    [SerializeField] private CharacterImageData[] characterImages;
    [SerializeField] private CharacterTutorialData[] characterTutorials;
    [SerializeField] private Image winScreenImage;
    [SerializeField] private Image deathScreenImage;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject characterSelectedPanel;
    [SerializeField] private InputActionReference resetPrefsAction;
    [SerializeField] private LevelSystem levelSystem;
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private PlayerDamage playerDamage;
    private bool openedFromMainMenu;
    private bool openedFromPause;
    private bool characterChosen;
    private int currentTutorialPage = 0;
    private int selectedCharacterIndex = 0;

    void Start()
    {
        if (PlayerPrefs.GetInt("SkipMainMenu", 0) == 1)
        {
            PlayerPrefs.DeleteKey("SkipMainMenu");
            characterChosen = true;
            PlayerPrefs.SetInt("CharacterChosen", 1);
            int retryLevel = PlayerPrefs.GetInt("RetryLevel", 1);
            PlayerPrefs.DeleteKey("RetryLevel");
            levelSystem.LoadLevel(retryLevel);
            StartGame();
            UpdateCharacterImages();
            return;
        }
        TimeScale(0);
        mainMenu.enabled = true;
        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        settings.enabled = false;
        characterSelect.enabled = false;
        levelSelect.enabled = false;
        deathScreen.enabled = false;
        winScreen.enabled = false;
        if (tutorialCanvas != null) tutorialCanvas.enabled = false;
        characterChosen = PlayerPrefs.GetInt("CharacterChosen", 0) == 1;
        if (characterSelectedPanel != null)
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
        UpdateCharacterImages();
        UpdateAttackState();
        SetupTutorialButtons();
    }

    private void SetupTutorialButtons()
    {
        if (leftArrowButton != null) leftArrowButton.onClick.RemoveAllListeners();
        if (rightArrowButton != null) rightArrowButton.onClick.RemoveAllListeners();
        if (exitTutorialButton != null) exitTutorialButton.onClick.RemoveAllListeners();

        if (leftArrowButton != null) leftArrowButton.onClick.AddListener(PreviousTutorialPage);
        if (rightArrowButton != null) rightArrowButton.onClick.AddListener(NextTutorialPage);
        if (exitTutorialButton != null) exitTutorialButton.onClick.AddListener(ExitTutorial);
    }

    public void OnLevelButtonClicked(int levelIndex)
    {
        if (levelIndex == 1 && PlayerPrefs.GetInt("TutorialShown", 0) == 0)
        {
            selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
            PlayerPrefs.SetInt("TutorialShown", 1);
            PlayerPrefs.Save();
            ShowTutorial();
        }
        else
        {
            levelSystem.LoadLevel(levelIndex);
            UpdateLevelButtons();
            levelSelect.enabled = false;
            StartGame();
        }
    }

    private void ShowTutorial()
    {
        TimeScale(0);
        levelSelect.enabled = false;
        if (tutorialCanvas != null)
        {
            tutorialCanvas.enabled = true;
            currentTutorialPage = 0;
            UpdateTutorialPage();
        }
    }

    public void NextTutorialPage()
    {
        int max = characterTutorials[selectedCharacterIndex].tutorialSprites.Length;
        if (currentTutorialPage < max - 1) currentTutorialPage++;
        UpdateTutorialPage();
    }

    public void PreviousTutorialPage()
    {
        if (currentTutorialPage > 0) currentTutorialPage--;
        UpdateTutorialPage();
    }

    private void UpdateTutorialPage()
    {
        if (tutorialImage == null || characterTutorials.Length == 0 || selectedCharacterIndex >= characterTutorials.Length) return;
        var sprites = characterTutorials[selectedCharacterIndex].tutorialSprites;
        if (currentTutorialPage >= sprites.Length) currentTutorialPage = sprites.Length - 1;
        if (currentTutorialPage < 0) currentTutorialPage = 0;
        tutorialImage.sprite = sprites[currentTutorialPage];

        if (leftArrowButton != null)
        {
            leftArrowButton.interactable = currentTutorialPage > 0;
            leftArrowButton.gameObject.SetActive(currentTutorialPage > 0);
        }
        if (rightArrowButton != null)
        {
            rightArrowButton.interactable = currentTutorialPage < sprites.Length - 1;
            rightArrowButton.gameObject.SetActive(currentTutorialPage < sprites.Length - 1);
        }
        if (exitTutorialButton != null)
            exitTutorialButton.gameObject.SetActive(currentTutorialPage == sprites.Length - 1);
    }

    public void ExitTutorial()
    {
        if (tutorialCanvas != null) tutorialCanvas.enabled = false;
        levelSystem.LoadLevel(1);
        UpdateLevelButtons();
        StartGame();
    }

    public void SetSelectedCharacterForTutorial(int characterIndex)
    {
        selectedCharacterIndex = characterIndex;
        PlayerPrefs.SetInt("SelectedCharacter", characterIndex);
        PlayerPrefs.Save();
        UpdateCharacterImages(); // Always refresh win/death images
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
        selectedCharacterIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        UpdateLevelButtons();
        UpdateCharacterImages(); // Refresh images every time
        UpdateAttackState();
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
        if (characterSelectedPanel != null) characterSelectedPanel.SetActive(false);
        if (tutorialCanvas != null) tutorialCanvas.enabled = false;
        if (levelSystem != null) levelSystem.ResetLevels();
        UpdateLevelButtons();
        UpdateAttackState();
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
        UpdateAttackState();
    }

    public void OnCharacterConfirmed()
    {
        characterChosen = true;
        PlayerPrefs.SetInt("CharacterChosen", 1);
        PlayerPrefs.Save();
        characterSelect.enabled = false;
        if (characterSelectedPanel != null) characterSelectedPanel.SetActive(true);
        UpdateCharacterImages();
        if (openedFromMainMenu) ShowLevelSelect();
        else GoToMainMenu();
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
        UpdateAttackState();
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

    public void StartGame()
    {
        TimeScale(1);
        inGameCanvas.enabled = true;
        pauzeMenu.enabled = false;
        mainMenu.enabled = false;
        settings.enabled = false;
        UpdateAttackState();
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
        UpdateAttackState();
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
        UpdateAttackState();
    }

    public void OpenSettingsFromMainMenu()
    {
        TimeScale(0);
        openedFromPause = false;
        mainMenu.enabled = false;
        settings.enabled = true;
        UpdateAttackState();
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
                UpdateAttackState();
            }
            else if (AllMenusOff())
            {
                TimeScale(0);
                pauzeMenu.enabled = true;
                UpdateAttackState();
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
        UpdateAttackState();
    }

    private bool AllMenusOff()
    {
        return !mainMenu.enabled && !settings.enabled && !characterSelect.enabled && !levelSelect.enabled &&
               !winScreen.enabled && !deathScreen.enabled && !tutorialCanvas.enabled;
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
        UpdateAttackState();
    }

    public void TimeScale(int scale)
    {
        Time.timeScale = scale;
    }

    public void RedoLevel()
    {
        Time.timeScale = 1f;
        if (levelSystem == null) return;
        PlayerPrefs.SetInt("RetryLevel", levelSystem.GetCurrentLevel());
        PlayerPrefs.SetInt("SkipMainMenu", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowWinScreen()
    {
        TimeScale(0);
        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        winScreen.enabled = true;
        UpdateCharacterImages();
        UpdateAttackState();
    }

    public void ShowDeathScreen()
    {
        TimeScale(0);
        inGameCanvas.enabled = false;
        pauzeMenu.enabled = false;
        deathScreen.enabled = true;
        UpdateCharacterImages();
        UpdateAttackState();
    }

    private void UpdateCharacterImages()
    {
        int index = PlayerPrefs.GetInt("SelectedCharacter", 0);
        if (index >= 0 && index < characterImages.Length)
        {
            if (winScreenImage != null) winScreenImage.sprite = characterImages[index].winSprite;
            if (deathScreenImage != null) deathScreenImage.sprite = characterImages[index].deathSprite;
        }
        else
        {
            if (winScreenImage != null) winScreenImage.sprite = null;
            if (deathScreenImage != null) deathScreenImage.sprite = null;
        }
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

    private void UpdateAttackState()
    {
        if (playerDamage != null)
            playerDamage.enabled = AllMenusOff() && inGameCanvas.enabled;
    }
}