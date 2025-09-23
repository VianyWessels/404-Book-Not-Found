using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    public KeyItem currentKeyInRange;
    public bool pickupOnEnter = false;
    public Hotbar hotbar; // assign your hotbar in the inspector

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null)
        {
            interactAction = playerInput.actions["Interact"]; // must exist in your Input Actions asset
        }
        else
        {
            Debug.LogWarning("PlayerInput component not found on this GameObject.");
        }
    }

    private void Update()
    {
        if (!pickupOnEnter && currentKeyInRange != null && interactAction != null && interactAction.WasPerformedThisFrame())
        {
            Pickup(currentKeyInRange);
        }
    }

    private void Pickup(KeyItem key)
    {
        if (key == null) return;

        // Ensure KeyInventory exists
        if (KeyInventory.Instance == null)
        {
            GameObject go = new GameObject("KeyInventory");
            go.AddComponent<KeyInventory>();
        }

        KeyInventory.Instance.AddKey(key.keyID);

        // Update hotbar
        if (hotbar != null)
        {
            hotbar.SetKey(key);
        }

        Destroy(key.gameObject);
        currentKeyInRange = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckKeyCollision(other.GetComponent<KeyItem>());
    }

    private void OnTriggerExit(Collider other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null && key == currentKeyInRange) currentKeyInRange = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckKeyCollision(other.GetComponent<KeyItem>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null && key == currentKeyInRange) currentKeyInRange = null;
    }

    private void CheckKeyCollision(KeyItem key)
    {
        if (key != null)
        {
            currentKeyInRange = key;
            if (pickupOnEnter) Pickup(key);
        }
    }
}
