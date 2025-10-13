using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public int attackDamage;
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
        {
            Hitbox hitboxScript = hitboxChild.GetComponent<Hitbox>();
            if (hitboxScript != null)
                hitboxScript.damage = attackDamage;

            hitboxChild.SetActive(true);
        }

        yield return new WaitForSeconds(hitboxDuration);
        if (hitboxChild != null)
            hitboxChild.SetActive(false);

        float weaponExtraDuration = 0.75f;
        yield return new WaitForSeconds(weaponExtraDuration);

        if (weapon != null)
            weapon.SetActive(false);
    }
}
