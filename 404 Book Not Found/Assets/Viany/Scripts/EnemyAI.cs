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
    public float deathDelay = 1.5f; // Time to wait after death before fading (1-2 seconds)
    public float fadeDuration = 1f; // Time to fade out
    public GameObject halfModel1; // Assign a pre-sliced half model prefab in Inspector
    public GameObject halfModel2; // Assign the other half prefab in Inspector
    public Vector3 separationForce = new Vector3(2f, 0f, 0f); // Force to push halves apart (adjust as needed)

    private Transform player;
    private float nextAttackTime = 0f;
    private int currentHealth;
    private Rigidbody rb;
    private bool isDead = false;
    private Renderer enemyRenderer; // For fading the original model

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
        currentHealth = maxHealth;

        // Add Rigidbody for falling physics
        rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = false; // Flying, so no gravity initially
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tumbling if desired

        // Get the renderer for fading (assumes the enemy has a MeshRenderer)
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer == null)
        {
            Debug.LogError("No Renderer found on enemy or its children! Fading won't work.");
        }
        else
        {
            // Set material to transparent for fading (if not already)
            SetMaterialTransparent(enemyRenderer.material);
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

        // Disable collider to prevent further interactions
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Handle slicing if half models are assigned
        if (halfModel1 != null && halfModel2 != null)
        {
            // Hide original renderer immediately
            if (enemyRenderer != null) enemyRenderer.enabled = false;

            // Instantiate halves
            GameObject h1 = Instantiate(halfModel1, transform.position, transform.rotation);
            GameObject h2 = Instantiate(halfModel2, transform.position, transform.rotation);

            // Add Rigidbodies to halves for falling
            Rigidbody rb1 = h1.AddComponent<Rigidbody>();
            Rigidbody rb2 = h2.AddComponent<Rigidbody>();
            rb1.useGravity = true;
            rb2.useGravity = true;

            // Apply separation force
            rb1.AddForce(separationForce, ForceMode.Impulse);
            rb2.AddForce(-separationForce, ForceMode.Impulse);

            // Ensure materials are set for fading on halves
            Renderer h1Renderer = h1.GetComponentInChildren<Renderer>();
            Renderer h2Renderer = h2.GetComponentInChildren<Renderer>();
            if (h1Renderer != null) SetMaterialTransparent(h1Renderer.material);
            if (h2Renderer != null) SetMaterialTransparent(h2Renderer.material);

            // Start fading and destruction on halves independently
            StartCoroutine(FadeAndDestroyAfterDelay(h1, deathDelay, fadeDuration));
            StartCoroutine(FadeAndDestroyAfterDelay(h2, deathDelay, fadeDuration));

            // Destroy original enemy object after spawning halves
            Destroy(gameObject);
        }
        else
        {
            // No slicing: Fall with original model
            if (rb != null)
            {
                rb.useGravity = true;
                rb.linearVelocity = Vector3.down * fallSpeedMultiplier; // Optional initial downward push
            }

            // Start fading after delay
            StartCoroutine(FadeAndDestroyAfterDelay(gameObject, deathDelay, fadeDuration));
        }
    }

    private IEnumerator FadeAndDestroyAfterDelay(GameObject obj, float delay, float fadeTime)
    {
        Debug.Log("Starting fade coroutine for: " + obj.name + " at " + Time.time);

        // Wait for the delay (1-2 seconds)
        yield return new WaitForSeconds(delay);

        // Get the renderer of the object (original or half)
        Renderer objRenderer = obj.GetComponentInChildren<Renderer>();
        if (objRenderer == null)
        {
            Debug.LogWarning("No renderer found on " + obj.name + ", forcing destruction at " + Time.time);
            Destroy(obj);
            yield break;
        }

        // Get the material to fade
        Material mat = objRenderer.material;
        if (mat == null)
        {
            Debug.LogWarning("No material found on " + obj.name + ", forcing destruction at " + Time.time);
            Destroy(obj);
            yield break;
        }

        // Store the original color
        Color startColor = mat.color;
        float elapsed = 0f;

        // Fade out over fadeDuration
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeTime);
            mat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ensure the object is fully transparent
        mat.color = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // Attempt to destroy the object
        Debug.Log("Attempting to destroy " + obj.name + " at " + Time.time);
        Destroy(obj);

        // Fallback: Check if still present and force destroy
        yield return new WaitForSeconds(0.1f);
        if (obj != null)
        {
            Debug.LogWarning("Forced destruction of " + obj.name + " at " + Time.time + " due to persistence");
            Destroy(obj);
        }
    }

    private void SetMaterialTransparent(Material mat)
    {
        if (mat == null) return;

        mat.SetFloat("_Mode", 3f); // Set to Fade mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: If you want to stop movement on ground hit, but since we're using a timer, not necessary
    }
}