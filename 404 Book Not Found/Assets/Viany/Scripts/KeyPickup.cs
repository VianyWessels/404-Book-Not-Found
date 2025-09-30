using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SphereCollider))]
public class KeyPickup : MonoBehaviour
{
    public KeyItem currentKeyInRange;
    public bool pickupOnEnter = false;
    public Hotbar hotbar;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction interactAction;
    [SerializeField] private float pickupRange = 3f;

    private SphereCollider trigger;

    private void Awake()
    {
        interactAction = playerInput.actions["Interact"];

        trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = pickupRange;
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

    private void CheckKeyCollision(KeyItem key)
    {
        if (key != null)
        {
            currentKeyInRange = key;
            if (pickupOnEnter) Pickup(key);
        }
    }
}
