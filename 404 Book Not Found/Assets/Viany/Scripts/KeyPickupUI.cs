using UnityEngine;
using TMPro;

public class KeyPickupUI : MonoBehaviour
{
    public GameObject uiPanel;
    public TextMeshProUGUI uiText;
    public string message = "Press E to pick up";

    private KeyPickup keyPickup;

    private void Start()
    {
        if (uiPanel != null) uiPanel.SetActive(false);
        keyPickup = GetComponent<KeyPickup>();
    }

    private void Update()
    {
        if (keyPickup == null) return;

        if (keyPickup.currentKeyInRange != null)
        {
            Debug.Log("Key in range! Popup zou nu zichtbaar moeten zijn.");
            if (uiPanel != null && !uiPanel.activeSelf) uiPanel.SetActive(true);
            if (uiText != null) uiText.text = message;
        }
        else
        {
            if (uiPanel != null && uiPanel.activeSelf) uiPanel.SetActive(false);
        }
    }

}
