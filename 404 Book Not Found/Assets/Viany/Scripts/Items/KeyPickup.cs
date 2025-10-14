using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class KeyPickup : MonoBehaviour
{
    public KeyItem currentKeyInRange;
    public bool pickupOnEnter = false;
    public Hotbar hotbar;
    public PlayerInput playerInput;
    private InputAction interactAction;

    private void Awake()
    {
        interactAction = playerInput.actions["Interact"];
    }

    private void Update()
    {
        if (!pickupOnEnter && currentKeyInRange && interactAction.WasPerformedThisFrame())
        {
            Pickup(currentKeyInRange);
        }
    }

    private void Pickup(KeyItem key)
    {
        string previousKeyID = hotbar.GetCurrentKey();
        if (!string.IsNullOrEmpty(previousKeyID))
        {
            KeyInventory.Instance.RemoveKey(previousKeyID);
            KeyItem previousKeyPrefab = KeyDatabase.Instance.GetKeyPrefab(previousKeyID);
            if (previousKeyPrefab != null)
            {
                Vector3 spawnPosition = key.transform.position;
                Quaternion spawnRotation = key.transform.rotation;
                KeyItem droppedKey = Instantiate(previousKeyPrefab, spawnPosition, spawnRotation);
                if (droppedKey.TryGetComponent(out KeyPickup keyPickup))
                {
                    keyPickup.playerInput = playerInput;
                    keyPickup.hotbar = hotbar;
                }
            }
        }
        KeyInventory.Instance.AddKey(key.keyID);
        hotbar.SetKey(key);
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
        if (key != null && key == currentKeyInRange)
        {
            currentKeyInRange = null;
        }
    }
}
