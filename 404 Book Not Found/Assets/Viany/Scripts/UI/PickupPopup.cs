using UnityEngine;
using TMPro;

public class PickupPopup : MonoBehaviour
{
    public Canvas popupCanvas;
    public TextMeshProUGUI popupText;
    public string message = "Press E to pick up";
    public float showDistance = 3f;
    public Transform player;

    void Start()
    {
        popupCanvas.enabled = false;

        popupText.text = message;
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= showDistance)
        {
            popupCanvas.enabled = true;
        }
        else
        {
            popupCanvas.enabled = false;
        }
    }
}
