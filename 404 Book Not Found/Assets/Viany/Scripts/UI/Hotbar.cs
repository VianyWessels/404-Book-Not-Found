using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Hotbar : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;
    public Canvas hotbarCanvas;
    public Sprite emptyImage;

    private string currentKeyID;

    public void Start()
    {
        Clear();
    }

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
        currentKeyID = null;
        label.text = "";
        icon.sprite = emptyImage;
    }
}
