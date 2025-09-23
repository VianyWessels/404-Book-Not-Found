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
        uiPanel.SetActive(false);
        keyPickup = GetComponent<KeyPickup>();
    }

    private void Update()
    {
        if (keyPickup == null) return;

        if (keyPickup.currentKeyInRange != null)
        {
            uiPanel.SetActive(true);
            uiText.text = message;
        }
        else
        {
            uiPanel.SetActive(false);
        }
    }
}
