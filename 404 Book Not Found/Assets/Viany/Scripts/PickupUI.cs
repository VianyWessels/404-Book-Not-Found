using UnityEngine;
using TMPro;

public class PickupUI : MonoBehaviour
{
    public GameObject uiPanel;
    public TextMeshProUGUI uiText;

    [SerializeField] private KeyPickup keyPickup;
    [SerializeField] private BookPickup bookPickup;

    private void Start()
    {
        uiPanel.SetActive(false);
    }

    private void Update()
    {
        if (keyPickup.currentKeyInRange != null)
        {
            uiPanel.SetActive(true);
            uiText.text = "Press E to pick up Key";
        }
        else if (bookPickup.currentBookInRange != null)
        {
            uiPanel.SetActive(true);
            uiText.text = "Press E to pick up Book";
        }
        else
        {
            uiPanel.SetActive(false);
        }
    }
}
