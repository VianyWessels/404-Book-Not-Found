using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;

    private string currentKeyID;

    public void SetKey(KeyItem key)
    {
        currentKeyID = key.keyID;

        label.text = currentKeyID;

        icon.sprite = key.keySprite;
        icon.enabled = true;
    }

    public string GetCurrentKey()
    {
        return currentKeyID;
    }

    public void Clear()
    {
        label.text = "";
        icon.enabled = false;
    }
}
