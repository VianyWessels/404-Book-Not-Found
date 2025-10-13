using System.Collections.Generic;
using UnityEngine;

public class KeyDatabase : MonoBehaviour
{
    public static KeyDatabase Instance { get; private set; }

    [System.Serializable]
    public struct KeyEntry
    {
        public string keyID;
        public KeyItem prefab;
    }

    public List<KeyEntry> keyEntries;
    private Dictionary<string, KeyItem> keyDict;

    private void Awake()
    {
        Instance = this;
        keyDict = new Dictionary<string, KeyItem>();
        foreach (var entry in keyEntries)
        {
            if (entry.prefab != null && !string.IsNullOrEmpty(entry.keyID))
                keyDict[entry.keyID] = entry.prefab;
        }
    }

    public KeyItem GetKeyPrefab(string keyID)
    {
        keyDict.TryGetValue(keyID, out KeyItem prefab);
        return prefab;
    }
}
