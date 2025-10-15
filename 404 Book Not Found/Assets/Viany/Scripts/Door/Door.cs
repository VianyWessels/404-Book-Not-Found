using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Door : MonoBehaviour
{
    public string requiredKeyID;
    public Transform doorPart;
    public float openRotation;
    public float openSpeed;
    public Transform teleportTarget;
    public Transform cameraTarget;
    public GameObject blackBackground;
    public Canvas popupCanvas;
    public TextMeshProUGUI popupText;
    public string unlockText = "Press E to open door";
    public Transform player;
    public PlayerInput playerInput;
    public Collider doorCollider;
    public Collider rangeCollider;
    public Collider teleportCollider;

    private InputAction interactAction;
    private bool isOpening;
    private bool isOpen;
    private bool playerPassedThrough;
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    private void Awake()
    {
        interactAction = playerInput.actions["Interact"];
        closedRotation = doorPart.rotation;
        targetRotation = closedRotation * Quaternion.Euler(0, openRotation, 0);
        popupCanvas.enabled = false;
        blackBackground.SetActive(false);
    }

    private void Update()
    {
        bool isPlayerInRange = rangeCollider.bounds.Contains(player.position);

        // Show popup if in range
        if (isPlayerInRange)
        {
            popupCanvas.enabled = true;
            popupText.text = KeyInventory.Instance.HasKey(requiredKeyID) ? unlockText : "";
        }
        else
        {
            popupCanvas.enabled = false;
        }

        // Handle door opening
        if (isPlayerInRange && !isOpening && !isOpen && KeyInventory.Instance.HasKey(requiredKeyID))
        {
            if (interactAction.WasPerformedThisFrame())
            {
                isOpening = true;
                popupCanvas.enabled = false;
                blackBackground.SetActive(true);
                KeyInventory.Instance.RemoveKey(requiredKeyID);
                player.GetComponentInChildren<Hotbar>()?.Clear();
                StartCoroutine(OpenDoor());
            }
        }

        // Teleport check using bounds (optional)
        if (isOpen && !playerPassedThrough && teleportCollider.bounds.Contains(player.position))
        {
            playerPassedThrough = true;
            StartCoroutine(TeleportPlayer());
        }
        else if (!teleportCollider.bounds.Contains(player.position))
        {
            playerPassedThrough = false;
        }
    }

    private IEnumerator OpenDoor()
    {
        doorCollider.enabled = false;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * openSpeed;
            doorPart.rotation = Quaternion.Slerp(closedRotation, targetRotation, t);
            yield return null;
        }
        doorCollider.enabled = true;
        isOpening = false;
        isOpen = true;
    }

    private IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(0.2f);
        player.position = teleportTarget.position;
        if (cameraTarget != null)
            Camera.main.transform.position = cameraTarget.position;
    }
}