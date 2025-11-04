using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage;
    public float attackCooldown;
    public float hitboxDuration;
    private float nextAttackTime;
    private bool isAttacking;

    public GameObject hitboxChild;
    public GameObject weapon;
    public Animator animator;
    public AudioManager audioManager;

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();

        if (hitboxChild != null)
        {
            var col = hitboxChild.GetComponent<BoxCollider>();
            if (col != null) col.isTrigger = true;
            hitboxChild.SetActive(false);
        }
        if (weapon != null) weapon.SetActive(false);
    }

    public void OnAttack(InputValue value)
    {
        if (!enabled || isAttacking || Time.time < nextAttackTime) return;

        if (playerHealth != null && playerHealth.isDead) return;

        if (value.isPressed)
            StartCoroutine(AttackSequence());
    }

    private IEnumerator AttackSequence()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        audioManager?.PlayAttack();
        animator?.SetTrigger("Attack");
        weapon?.SetActive(true);

        if (hitboxChild != null)
        {
            var hitbox = hitboxChild.GetComponent<Hitbox>();
            if (hitbox != null) hitbox.damage = attackDamage;
            hitboxChild.SetActive(true);
        }

        yield return new WaitForSeconds(hitboxDuration);
        if (hitboxChild != null) hitboxChild.SetActive(false);

        yield return new WaitForSeconds(0.75f);
        if (weapon != null) weapon.SetActive(false);

        isAttacking = false;
    }
}