using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private Transform characterSpawnPoint;
    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private Button confirmButton;
    [SerializeField] private UIController uiController;
    [SerializeField] private float rotationSpeed = 20f;

    private GameObject currentCharacter;
    private int currentIndex;
    private float currentYRotation;

    void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedCharacter", 0);
        SpawnCharacter();

        leftArrow.onClick.AddListener(PrevCharacter);
        rightArrow.onClick.AddListener(NextCharacter);
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void Update()
    {
        currentYRotation += rotationSpeed * Time.unscaledDeltaTime;
        currentCharacter.transform.rotation = Quaternion.Euler(0f, currentYRotation, 0f);
    }

    void PrevCharacter()
    {
        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = characterPrefabs.Length - 1;
        }
        SpawnCharacter();
    }

    void NextCharacter()
    {
        currentIndex++;
        if (currentIndex >= characterPrefabs.Length)
        {
            currentIndex = 0;
        }
        SpawnCharacter();
    }

    void SpawnCharacter()
    {
        if (currentCharacter != null)
        {
            Destroy(currentCharacter);
        }

        currentCharacter = Instantiate(characterPrefabs[currentIndex], characterSpawnPoint.position, Quaternion.identity);
        currentCharacter.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        currentYRotation = 180f;
    }

    void ConfirmSelection()
    {
        PlayerPrefs.SetInt("SelectedCharacter", currentIndex);
        PlayerPrefs.Save();
    }
}
