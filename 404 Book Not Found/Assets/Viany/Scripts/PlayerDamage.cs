using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage;
    public int hitboxDamage;

    public float attackRange;
    public float attackCooldown;
    public float hitboxDuration;
    private float nextAttackTime;

    public GameObject hitboxChild;

    void Start()
    {
        if (hitboxChild != null)
        {
            BoxCollider hitbox = hitboxChild.GetComponent<BoxCollider>();
            hitbox.isTrigger = true;

            hitboxChild.SetActive(false);
        }
    }

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
                enemy.TakeDamage(attackDamage);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitboxChild != null && other.gameObject == hitboxChild) return;

        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemy = other.GetComponent<EnemyAI>();
            enemy.TakeDamage(hitboxDamage);
        }
    }
}
