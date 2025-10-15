using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] levelObjects;

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
    }

    public void ActivateCurrentLevel()
    {
        HideAllLevels();

        int index = currentLevel - 1;
        if (levelObjects == null || levelObjects.Length == 0) return;
        if (index >= 0 && index < levelObjects.Length)
            levelObjects[index]?.SetActive(true);
    }

    public void HideAllLevels()
    {
        if (levelObjects == null) return;
        foreach (var lvl in levelObjects)
            if (lvl != null)
                lvl.SetActive(false);
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
