using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage = 50; // Damage for manual attack (Spacebar)
    public float attackRange = 2f; // Range of the manual slash attack
    public float attackCooldown = 1f; // Time between manual attacks
    public GameObject hitboxChild; // Assign the hitbox child GameObject in Inspector
    public int hitboxDamage = 1; // Damage dealt by hitbox (set to 1 to match EnemyAI maxHealth)

    private PlayerHealth health;
    private float nextAttackTime = 0f;

    void Start()
    {
        health = GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogError("PlayerHealth component not found on the player!");
        }

        if (hitboxChild == null)
        {
            Debug.LogError("Hitbox child not assigned in Inspector! Please assign the hitbox GameObject.");
        }
        else
        {
            // Ensure the hitbox child has a BoxCollider set as trigger
            BoxCollider hitbox = hitboxChild.GetComponent<BoxCollider>();
            if (hitbox == null)
            {
                Debug.LogError("Hitbox child does not have a BoxCollider!");
            }
            else if (!hitbox.isTrigger)
            {
                Debug.LogWarning("Hitbox child's BoxCollider is not set as a trigger. Please enable 'Is Trigger' in the Inspector.");
                hitbox.isTrigger = true; // Attempt to set it, but manual check is better
            }
        }
    }

    // Called by the Input System when the attack action is performed
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage(int amount)
    {
        if (health != null)
        {
            health.TakeDamage(amount);
        }
    }

    private void Attack()
    {
        // Perform a slash attack using overlap sphere for manual attack
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * (attackRange / 2f), attackRange / 2f);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                }
            }
        }
        Debug.Log("Player attacked!");
    }

    // Handle trigger events from the hitbox child
    private void OnTriggerEnter(Collider other)
    {
        if (hitboxChild != null && other.gameObject == hitboxChild)
        {
            // This is a safeguard to ensure the trigger is from the hitbox child
            return;
        }

        // Check if the entering object is an enemy
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
            {
                enemy.TakeDamage(hitboxDamage); // Deal damage to kill the enemy
                Debug.Log("Enemy entered hitbox and took damage at " + Time.time);
            }
        }
    }
}