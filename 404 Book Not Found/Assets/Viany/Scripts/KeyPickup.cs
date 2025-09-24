using UnityEngine;
using UnityEngine.InputSystem;

public class KeyPickup : MonoBehaviour
{
    public KeyItem currentKeyInRange;
    public bool pickupOnEnter = false;
    public Hotbar hotbar;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction interactAction;

    private void Awake()
    {
        interactAction = playerInput.actions["Interact"];
    }

    private void Update()
    {
        if (!pickupOnEnter && currentKeyInRange && interactAction != null && interactAction.WasPerformedThisFrame())
        {
            Pickup(currentKeyInRange);
        }
    }

    private void Pickup(KeyItem key)
    {
        KeyInventory.Instance.AddKey(key.keyID);

        hotbar.SetKey(key);

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
