using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed of the flying enemy
    public float attackRange = 1f; // Range to attack the player
    public int attackDamage = 10; // Damage dealt to player
    public float attackCooldown = 2f; // Time between attacks
    public float bobAmplitude = 0.5f; // Amplitude of bobbing motion for flying effect
    public float bobFrequency = 2f; // Frequency of bobbing
    public int maxHealth = 1; // Health set to 1 for one-hit kill
    public float fallSpeedMultiplier = 2f; // Optional: Make falling faster if needed
    public float minFallTime = 3f; // Increased to 3 seconds to allow more fall time

    private Transform player;
    private float nextAttackTime = 0f;
    private int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    private Renderer enemyRenderer; // For hiding the original model
    private Collider mainCollider; // To manage the enemy's collider
    private bool hasFallen = false; // Track if the enemy has landed

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
        currentHealth = maxHealth;

        // Use existing Rigidbody or add if none
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            Debug.Log("Added new Rigidbody with constraints: 0, mass: 1");
        }
        else
        {
            Debug.Log("Using existing Rigidbody with constraints: " + rb.constraints + ", mass: " + rb.mass);
        }
        rb.useGravity = false; // Flying, so no gravity initially
        rb.constraints &= ~RigidbodyConstraints.FreezeRotation; // Clear FreezeRotation
        rb.mass = 1f; // Set a reasonable mass

        // Get the renderer and main collider
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer == null)
        {
            Debug.LogError("No Renderer found on enemy or its children!");
        }
        mainCollider = GetComponent<Collider>();
        if (mainCollider == null)
        {
            Debug.LogError("No Collider found on enemy! Add a Collider (e.g., CapsuleCollider).");
        }
        else
        {
            Debug.Log("Collider found: " + mainCollider.GetType().Name);
        }
    }

    void Update()
    {
        if (isDead || player == null) return;

        // Move towards the player
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Add bobbing for flying effect
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);

        // Look at the player
        transform.LookAt(player);

        // Attack if in range
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        PlayerDamage playerDamage = player.GetComponent<PlayerDamage>();
        if (playerDamage != null)
        {
            playerDamage.TakeDamage(attackDamage);
            Debug.Log("Enemy attacked player!");
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Enemy died at " + Time.time);

        // Hide original renderer immediately (keep collider for collision)
        if (enemyRenderer != null)
        {
            enemyRenderer.enabled = false;
            Debug.Log("Renderer disabled on death.");
        }
        else
        {
            Debug.LogWarning("No renderer to disable on death!");
        }

        // Enable ragdoll-like fall
        if (rb != null)
        {
            rb.useGravity = true;
            Debug.Log("Gravity enabled on Rigidbody: " + rb.useGravity);
            rb.linearVelocity = Vector3.down * fallSpeedMultiplier; // Initial downward push
            rb.angularVelocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); // Add random rotation
            Debug.Log("Rigidbody velocity set to: " + rb.linearVelocity + ", angularVelocity: " + rb.angularVelocity);
        }
        else
        {
            Debug.LogError("Rigidbody is null on death!");
        }

        // Start fall monitoring
        StartCoroutine(MonitorFallAndDestroy());
    }

    private IEnumerator MonitorFallAndDestroy()
    {
        float fallStartTime = Time.time;
        Debug.Log("Starting fall monitoring for: " + gameObject.name + " at " + fallStartTime);

        // Wait for at least minFallTime
        yield return new WaitForSeconds(minFallTime);

        // Check if the enemy has hit the ground or time has passed
        if (!hasFallen)
        {
            Debug.LogWarning("Enemy did not detect ground collision, forcing destruction after minFallTime at " + Time.time);
        }
        else
        {
            Debug.Log("Enemy landed, proceeding to destroy at " + Time.time);
        }

        // Attempt to destroy the object
        Debug.Log("Attempting to destroy " + gameObject.name + " at " + Time.time);
        Destroy(gameObject);

        // Fallback: Check if still present and force destroy
        yield return new WaitForSeconds(0.1f);
        if (gameObject != null)
        {
            Debug.LogWarning("Forced destruction of " + gameObject.name + " at " + Time.time + " due to persistence");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detect when the enemy hits the ground
        if (!hasFallen && collision.gameObject.CompareTag("Ground"))
        {
            hasFallen = true;
            Debug.Log("Enemy collided with ground at " + Time.time);
        }
    }
}