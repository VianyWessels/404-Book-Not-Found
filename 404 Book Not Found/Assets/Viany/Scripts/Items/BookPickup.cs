using UnityEngine;
using UnityEngine.InputSystem;

public class BookPickup : MonoBehaviour
{
    public BookItem currentBookInRange;
    public bool pickupOnEnter;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private InputAction interactAction;

    private void Awake()
    {
        interactAction = playerInput.actions["Interact"];
    }

    private void Update()
    {
        if (!pickupOnEnter && currentBookInRange && interactAction.WasPerformedThisFrame())
        {
            Pickup(currentBookInRange);
        }
    }

    private void Pickup(BookItem book)
    {
        if (book == null)
        {
            return;
        }

        Destroy(book.gameObject);
        currentBookInRange = null;

        Win();
    }

    private void OnTriggerEnter(Collider other)
    {
        var book = other.GetComponent<BookItem>();
        if (book != null)
        {
            currentBookInRange = book;
            if (pickupOnEnter)
            {
                Pickup(book);
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        var book = other.GetComponent<BookItem>();
        if (book != null && book == currentBookInRange)
        {
            currentBookInRange = null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var book = other.GetComponent<BookItem>();
        if (book != null)
        {
            currentBookInRange = book;
            if (pickupOnEnter) Pickup(book);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var book = other.GetComponent<BookItem>();
        if (book != null && book == currentBookInRange)
        {
            currentBookInRange = null;
        }
    }

    private void Win()
    {
        Time.timeScale = 0f;
        // add win functions
    }
}
