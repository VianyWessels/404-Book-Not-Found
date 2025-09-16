using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage = 50; // Adjust as needed; since enemies die in one hit, any positive value works
    public float attackRange = 2f; // Range of the slash attack
    public float attackCooldown = 1f; // Time between attacks

    private PlayerHealth health;
    private float nextAttackTime = 0f;

    void Start()
    {
        health = GetComponent<PlayerHealth>();
        if (health == null)
        {
            Debug.LogError("PlayerHealth component not found on the player!");
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
        // Perform a slash attack using overlap sphere for simplicity (assumes 3D game)
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
}