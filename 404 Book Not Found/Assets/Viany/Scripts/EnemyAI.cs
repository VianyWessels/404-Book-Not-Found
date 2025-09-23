using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement & Combat")]
    public float moveSpeed = 5f;
    public float attackRange = 1f;
    public int attackDamage = 1;        // Damage to player per attack
    public float attackCooldown = 2f;
    public float bobAmplitude = 0.5f;
    public float bobFrequency = 2f;
    public int maxHealth = 1;
    public float detectionRange = 10f;

    [Header("Death / Falling")]
    public float fallSpeedMultiplier = 2f;
    public float minFallTime = 3f;
    public float splitForce = 3f;
    public float bounceForce = 1.5f;
    public GameObject leftHalf;
    public GameObject rightHalf;

    private Transform player;
    private float nextAttackTime = 0f;
    private int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    private Collider mainCollider;
    private Renderer enemyRenderer;
    private bool leftBounced = false;
    private bool rightBounced = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");

        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.constraints &= ~RigidbodyConstraints.FreezeRotation;
        rb.mass = 1f;

        mainCollider = GetComponent<Collider>();
        if (mainCollider == null)
            Debug.LogError("No Collider found on enemy!");

        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer == null)
            Debug.LogWarning("No renderer found on enemy or children.");

        if (leftHalf != null) leftHalf.SetActive(false);
        if (rightHalf != null) rightHalf.SetActive(false);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            IdleBehavior();
        }
        else
        {
            MoveAndAttackPlayer(distance);
        }
    }

    private void IdleBehavior()
    {
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);
        transform.Rotate(Vector3.up * Time.deltaTime * 20f);
    }

    private void MoveAndAttackPlayer(float distance)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);

        transform.LookAt(player);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1); // Enemy deals 1 damage per attack
            Debug.Log("Enemy damaged player! Current health: " + playerHealth.GetCurrentHealth());
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found on the player!");
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        if (mainCollider != null)
            mainCollider.enabled = false;

        // Split into halves
        if (leftHalf != null)
        {
            leftHalf.SetActive(true);
            Rigidbody rbLeft = leftHalf.GetComponent<Rigidbody>();
            if (rbLeft != null)
            {
                rbLeft.isKinematic = false;
                rbLeft.linearVelocity = (Vector3.left + Vector3.up) * splitForce;
                rbLeft.angularVelocity = new Vector3(Random.value, Random.value, Random.value);
            }
        }

        if (rightHalf != null)
        {
            rightHalf.SetActive(true);
            Rigidbody rbRight = rightHalf.GetComponent<Rigidbody>();
            if (rbRight != null)
            {
                rbRight.isKinematic = false;
                rbRight.linearVelocity = (Vector3.right + Vector3.up) * splitForce;
                rbRight.angularVelocity = new Vector3(Random.value, Random.value, Random.value);
            }
        }

        rb.useGravity = false; // Main enemy body stays
        StartCoroutine(MonitorFallAndDestroy());
    }

    private IEnumerator MonitorFallAndDestroy()
    {
        yield return new WaitForSeconds(minFallTime);
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (leftHalf != null && !leftBounced)
            {
                Rigidbody rbLeft = leftHalf.GetComponent<Rigidbody>();
                if (rbLeft != null)
                    rbLeft.linearVelocity = new Vector3(rbLeft.linearVelocity.x, bounceForce, rbLeft.linearVelocity.z);
                leftBounced = true;
            }

            if (rightHalf != null && !rightBounced)
            {
                Rigidbody rbRight = rightHalf.GetComponent<Rigidbody>();
                if (rbRight != null)
                    rbRight.linearVelocity = new Vector3(rbRight.linearVelocity.x, bounceForce, rbRight.linearVelocity.z);
                rightBounced = true;
            }
        }
    }
}
