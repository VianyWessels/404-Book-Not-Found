using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyInventory : MonoBehaviour
{
    public static KeyInventory Instance { get; private set; }
    private HashSet<string> keys = new HashSet<string>();
    public event Action<string> OnKeyCollected;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasKey(string id)
    {
        return keys.Contains(id);
    }

    public void AddKey(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }
        if (keys.Add(id))
        {
            OnKeyCollected?.Invoke(id);
        }
    }

    public bool RemoveKey(string id)
    {
        return keys.Remove(id);
    }

    public void Clear()
    {
        keys.Clear();
    }
}
