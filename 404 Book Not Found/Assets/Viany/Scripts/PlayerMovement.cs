using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 30f; // Adjust this for faster/slower movement
    public float dragAmount = 10f; // Higher value = quicker stop (try 2-10 for "a little bit of drag")
    public float rotationSpeed = 15f; // Speed of rotation (adjust in Inspector)

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = dragAmount; // Applies friction to smooth out stopping
    }

    // This method is automatically called by the Player Input component
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // Convert 2D input to 3D world movement (XZ plane for top-down)
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        rb.AddForce(moveDirection * moveSpeed);

        // Rotate to face movement direction
        if (moveDirection.magnitude > 0.1f) // Only rotate if there's significant input
        {
            Vector3 targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }
}