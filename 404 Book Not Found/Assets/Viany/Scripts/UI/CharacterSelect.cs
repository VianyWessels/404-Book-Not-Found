using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private string[] characterNames;
    [SerializeField] private string[] characterDescriptions;
    [SerializeField] private Sprite[] characterImages;
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text mainMenuCharacterNameText;
    [SerializeField] private TMP_Text characterDescription;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Canvas characterSelectCanvas;
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float slideDistance = 3f;

    private GameObject[] characters;
    private int currentIndex;
    private float currentYRotation;
    private bool isSliding;

    void Start()
    {
        characters = new GameObject[characterPrefabs.Length];
        for (int i = 0; i < characterPrefabs.Length; i++)
        {
            characters[i] = Instantiate(characterPrefabs[i], characterSpawnPoint.position, Quaternion.Euler(0f, 180f, 0f));
            characters[i].SetActive(false);
        }

        currentIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        characters[currentIndex].SetActive(true);
        currentYRotation = 180f;

        UpdateCharacterUI();
        UpdateMainMenuCharacterName();

        leftArrow.onClick.AddListener(() => ChangeCharacter(-1));
        rightArrow.onClick.AddListener(() => ChangeCharacter(1));
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void Update()
    {
        if (characters[currentIndex].activeSelf && characterSelectCanvas != null && characterSelectCanvas.enabled && !isSliding)
        {
            currentYRotation += rotationSpeed * Time.unscaledDeltaTime;
            characters[currentIndex].transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
        }
    }

    private void ChangeCharacter(int direction)
    {
        if (isSliding) return;

        int newIndex = currentIndex + direction;
        if (newIndex < 0) newIndex = characters.Length - 1;
        if (newIndex >= characters.Length) newIndex = 0;

        StartCoroutine(SlideToCharacter(newIndex, direction));
    }

    private IEnumerator SlideToCharacter(int newIndex, int direction)
    {
        isSliding = true;

        GameObject oldCharacter = characters[currentIndex];
        GameObject newCharacter = characters[newIndex];

        newCharacter.transform.position = characterSpawnPoint.position + Vector3.right * direction * slideDistance;
        newCharacter.SetActive(true);

        float startTime = Time.unscaledTime;
        Vector3 startOld = oldCharacter.transform.position;
        Vector3 endOld = oldCharacter.transform.position - Vector3.right * direction * slideDistance;
        Vector3 startNew = newCharacter.transform.position;
        Vector3 endNew = characterSpawnPoint.position;

        while (Time.unscaledTime - startTime < slideDuration)
        {
            float t = (Time.unscaledTime - startTime) / slideDuration;
            oldCharacter.transform.position = Vector3.Lerp(startOld, endOld, t);
            newCharacter.transform.position = Vector3.Lerp(startNew, endNew, t);
            yield return null;
        }

        oldCharacter.SetActive(false);
        currentIndex = newIndex;
        currentYRotation = 180f;

        UpdateCharacterUI();
        isSliding = false;
    }

    private void UpdateCharacterUI()
    {
        if (characterNames.Length > currentIndex)
        {
            characterNameText.text = characterNames[currentIndex];
            characterDescription.text = characterDescriptions[currentIndex];

            if (characterImages.Length > currentIndex && characterIcon != null)
                characterIcon.sprite = characterImages[currentIndex];
        }
    }

    private void UpdateMainMenuCharacterName()
    {
        if (characterNames.Length > currentIndex)
        {
            mainMenuCharacterNameText.text = characterNames[currentIndex];
        }
    }

    private void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.Save();
        UpdateMainMenuCharacterName();
    }
}
