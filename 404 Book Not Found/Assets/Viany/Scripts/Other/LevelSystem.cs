using UnityEngine;

public class LevelSystem : MonoBehaviour
{
    private const int TotalLevels = 5;
    private const string LevelKeyPrefix = "LevelUnlocked_";

    void Start()
    {
        if (!IsLevelUnlocked(1))
        {
            UnlockLevel(1);
        }
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
    public void UnlockNextLevel(int currentLevel)
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel <= TotalLevels)
        {
            UnlockLevel(nextLevel);
        }
    }
    public int GetUnlockedLevelCount()
    {
        int count = 0;
        for (int i = 1; i <= TotalLevels; i++)
        {
            if (IsLevelUnlocked(i)) count++;
        }
        return count;
    }
    public void ResetLevels()
    {
        for (int i = 1; i <= TotalLevels; i++)
        {
            PlayerPrefs.SetInt(LevelKeyPrefix + i, 0);
        }
        PlayerPrefs.Save();
        UnlockLevel(1);
    }
}
