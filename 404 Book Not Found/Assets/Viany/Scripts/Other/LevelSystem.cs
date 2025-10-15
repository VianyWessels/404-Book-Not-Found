using UnityEngine;

[System.Serializable]
public struct LevelData
{
    public GameObject levelObject;
    public Transform playerSpawnTransform;
    public Transform cameraTransform;
}

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private LevelData[] levels;
    private const int TotalLevels = 5;
    private const string LevelKeyPrefix = "LevelUnlocked_";
    private const string CurrentLevelKey = "CurrentLevel";
    private int currentLevel = 1;

    private void Start()
    {
        if (!IsLevelUnlocked(1)) UnlockLevel(1);
        currentLevel = PlayerPrefs.GetInt(CurrentLevelKey, 1);
        ActivateCurrentLevel();
    }

    public bool IsLevelUnlocked(int level)
    {
        if (level < 1 || level > TotalLevels) return false;
        return PlayerPrefs.GetInt(LevelKeyPrefix + level, 0) == 1;
    }

    public void UnlockLevel(int level)
    {
        if (level < 1 || level > TotalLevels) return;
        PlayerPrefs.SetInt(LevelKeyPrefix + level, 1);
        PlayerPrefs.Save();
    }

    public void UnlockNextLevel(int level)
    {
        int nextLevel = level + 1;
        if (nextLevel <= TotalLevels)
            UnlockLevel(nextLevel);
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetTotalLevels() => TotalLevels;

    public void LoadLevel(int levelNumber)
    {
        if (!IsLevelUnlocked(levelNumber)) return;
        currentLevel = levelNumber;
        PlayerPrefs.SetInt(CurrentLevelKey, currentLevel);
        PlayerPrefs.Save();
        ActivateCurrentLevel();
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.ResetHealth();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Camera mainCamera = Camera.main;
        int index = currentLevel - 1;
        if (index >= 0 && index < levels.Length)
        {
            if (player != null && levels[index].playerSpawnTransform != null)
            {
                player.transform.position = levels[index].playerSpawnTransform.position;
                player.transform.rotation = levels[index].playerSpawnTransform.rotation;
            }
            if (mainCamera != null && levels[index].cameraTransform != null)
            {
                mainCamera.transform.position = new Vector3(
                    levels[index].cameraTransform.position.x,
                    levels[index].cameraTransform.position.y,
                    mainCamera.transform.position.z);
                mainCamera.transform.rotation = levels[index].cameraTransform.rotation;
            }
        }
    }

    public void ActivateCurrentLevel()
    {
        HideAllLevels();
        int index = currentLevel - 1;
        if (levels == null || levels.Length == 0) return;
        if (index >= 0 && index < levels.Length)
            levels[index].levelObject?.SetActive(true);
    }

    public void HideAllLevels()
    {
        if (levels == null) return;
        foreach (var level in levels)
            if (level.levelObject != null)
                level.levelObject.SetActive(false);
    }

    public void ResetLevels()
    {
        for (int i = 1; i <= TotalLevels; i++)
            PlayerPrefs.SetInt(LevelKeyPrefix + i, 0);
        PlayerPrefs.Save();
        UnlockLevel(1);
        currentLevel = 1;
        ActivateCurrentLevel();
    }
}