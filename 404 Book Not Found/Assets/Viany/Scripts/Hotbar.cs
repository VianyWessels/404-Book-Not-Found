using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public Image icon;              // The UI Image for the key sprite
    public TextMeshProUGUI label;   // Optional: show key name

    private string currentKeyID;

    public void SetKey(KeyItem key)
    {
        if (key == null) return;

        currentKeyID = key.keyID;

        if (label != null)
            label.text = currentKeyID;

        if (icon != null)
        {
            icon.sprite = key.keySprite; // Set the sprite of the UI Image
            icon.enabled = true;
        }
    }

    public string GetCurrentKey()
    {
        return currentKeyID;
    }

    public void Clear()
    {
        currentKeyID = null;

        if (label != null)
            label.text = "";

        if (icon != null)
            icon.enabled = false;
    }
}
