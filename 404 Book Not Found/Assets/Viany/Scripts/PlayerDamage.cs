using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    public int attackDamage = 50;          // Manual attack damage
    public float attackRange = 2f;         // Range of manual attack
    public float attackCooldown = 1f;      // Cooldown for attacks
    public GameObject hitboxChild;         // Assign hitbox GameObject
    public int hitboxDamage = 1;           // Damage dealt by hitbox
    public float hitboxDuration = 1f;      // How long hitbox stays active

    private float nextAttackTime = 0f;

    void Start()
    {
        if (hitboxChild != null)
        {
            BoxCollider hitbox = hitboxChild.GetComponent<BoxCollider>();
            if (hitbox != null)
                hitbox.isTrigger = true;

            hitboxChild.SetActive(false);
        }
    }

    // Called by Input System
    public void OnAttack(InputValue value)
    {
        if (value.isPressed && Time.time >= nextAttackTime)
        {
            StartCoroutine(ActivateHitbox());
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator ActivateHitbox()
    {
        if (hitboxChild == null) yield break;

        hitboxChild.SetActive(true);
        yield return new WaitForSeconds(hitboxDuration);
        hitboxChild.SetActive(false);
    }

    private void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * (attackRange / 2f), attackRange / 2f);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                EnemyAI enemy = hitCollider.GetComponent<EnemyAI>();
                if (enemy != null)
                    enemy.TakeDamage(attackDamage);
            }
        }
        Debug.Log("Player attacked!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitboxChild != null && other.gameObject == hitboxChild) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            if (enemy != null)
                enemy.TakeDamage(hitboxDamage);
        }
    }
}
