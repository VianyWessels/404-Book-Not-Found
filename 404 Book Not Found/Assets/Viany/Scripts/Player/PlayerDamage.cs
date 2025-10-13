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
    public GameObject weapon;
    public Animator animator;

    void Start()
    {
        if (hitboxChild != null)
        {
            BoxCollider hitbox = hitboxChild.GetComponent<BoxCollider>();
            hitbox.isTrigger = true;
            hitboxChild.SetActive(false);
        }

        if (weapon != null)
            weapon.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed && Time.time >= nextAttackTime)
        {
            animator.SetTrigger("Attack");
            StartCoroutine(AttackSequence());
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private IEnumerator AttackSequence()
    {
        if (weapon != null)
            weapon.SetActive(true);

        if (hitboxChild != null)
            hitboxChild.SetActive(true);

        yield return new WaitForSeconds(hitboxDuration);

        if (hitboxChild != null)
            hitboxChild.SetActive(false);

        yield return new WaitForSeconds(0.75f);
        if (weapon != null)
            weapon.SetActive(false);
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
