using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 20f;
    public float dragAmount = 5f;
    public float rotationSpeed = 15f;

    [Header("Camera Follow")]
    public Transform cam;        // Assign your Camera here
    public Vector3 camOffset;    // Offset from the player
    public float camFollowSpeed = 10f;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;
    private Quaternion camLockedRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = dragAmount;

        if (cam != null)
            camLockedRotation = cam.rotation; // Store initial camera rotation
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        rb.AddForce(moveDirection * moveSpeed);

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 targetDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
        }
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // Smooth camera follow
        Vector3 targetPos = transform.position + camOffset;
        cam.position = Vector3.Lerp(cam.position, targetPos, camFollowSpeed * Time.deltaTime);

        // Keep rotation locked
        cam.rotation = camLockedRotation;
    }
}
