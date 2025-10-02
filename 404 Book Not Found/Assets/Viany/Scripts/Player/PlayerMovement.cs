using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float dragAmount;
    public float rotationSpeed;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = dragAmount;
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
}
