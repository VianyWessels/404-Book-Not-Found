using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    public KeyItem currentKeyInRange;
    public bool pickupOnEnter = false;

    private PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        interactAction = playerInput.actions["Interact"]; // moet bestaan in je Input Actions
    }

    private void Update()
    {
        if (!pickupOnEnter && currentKeyInRange != null && interactAction.WasPerformedThisFrame())
        {
            Pickup(currentKeyInRange);
        }
    }

    private void Pickup(KeyItem key)
    {
        if (key == null) return;
        if (KeyInventory.Instance == null)
        {
            var go = new GameObject("KeyInventory");
            go.AddComponent<KeyInventory>();
        }
        KeyInventory.Instance.AddKey(key.keyID);
        Destroy(key.gameObject);
        currentKeyInRange = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null)
        {
            currentKeyInRange = key;
            if (pickupOnEnter) Pickup(key);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null && key == currentKeyInRange) currentKeyInRange = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null)
        {
            currentKeyInRange = key;
            if (pickupOnEnter) Pickup(key);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var key = other.GetComponent<KeyItem>();
        if (key != null && key == currentKeyInRange) currentKeyInRange = null;
    }
}
