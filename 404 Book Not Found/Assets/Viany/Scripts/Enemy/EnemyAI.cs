using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed;
    public float attackRange;
    public float bobAmplitude;
    public float bobFrequency;
    public float detectionRange;
    public float minY;
    public float maxY;

    public int attackDamage;
    public float attackCooldown;

    public int maxHealth;
    public float splitForce;
    public float bounceForce;
    public float minFallTime;

    public GameObject leftHalf;
    public GameObject rightHalf;
    public GameObject splitHalo;
    public PlayerHealth playerHealth;
    public Transform player;
    public Rigidbody rb;
    public Rigidbody rbLeft;
    public Rigidbody rbRight;
    public Rigidbody rbSplitHalo;
    public Collider mainCollider;
    public Renderer enemyRenderer;
    public Renderer haloRenderer;

    private float nextAttackTime;
    private int currentHealth;
    private bool isDead;
    private bool leftBounced;
    private bool rightBounced;
    private bool haloBounced;

    void Start()
    {
        currentHealth = maxHealth;

        if (leftHalf != null) leftHalf.SetActive(false);
        if (rightHalf != null) rightHalf.SetActive(false);
        if (splitHalo != null) splitHalo.SetActive(false);
    }

    void FixedUpdate()
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

        Vector3 clampedPos = rb.position;
        clampedPos.y = Mathf.Clamp(clampedPos.y, minY, maxY);
        rb.position = clampedPos;
    }

    private void IdleBehavior()
    {
        if (rb == null) return;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        Vector3 newPos = rb.position + new Vector3(0f, bob * Time.fixedDeltaTime, 0f);
        rb.MovePosition(newPos);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, 20f * Time.fixedDeltaTime, 0f));
    }

    private void MoveAndAttackPlayer(float distance)
    {
        if (rb == null) return;

        Vector3 direction = (player.position - rb.position).normalized;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        Vector3 move = direction * moveSpeed * Time.fixedDeltaTime;
        move.y += bob * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        Vector3 lookDir = player.position - rb.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, 5f * Time.fixedDeltaTime));
        }

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        if (playerHealth != null)
            playerHealth.TakeDamage(attackDamage);
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

        if (enemyRenderer != null) enemyRenderer.enabled = false;
        if (haloRenderer != null) haloRenderer.enabled = false;
        if (mainCollider != null) mainCollider.enabled = false;

        if (leftHalf != null) leftHalf.SetActive(true);
        if (rightHalf != null) rightHalf.SetActive(true);
        if (splitHalo != null) splitHalo.SetActive(true);

        if (rbLeft != null)
        {
            rbLeft.linearVelocity = (Vector3.left + Vector3.up * 0.5f) * splitForce;
            rbLeft.angularVelocity = Random.onUnitSphere * 5f;
        }

        if (rbRight != null)
        {
            rbRight.linearVelocity = (Vector3.right + Vector3.up * 0.5f) * splitForce;
            rbRight.angularVelocity = Random.onUnitSphere * 5f;
        }

        if (rbSplitHalo != null)
        {
            rbSplitHalo.linearVelocity = (Vector3.up + Vector3.forward * 0.5f) * splitForce;
            rbSplitHalo.angularVelocity = Random.onUnitSphere * 5f;
        }

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
            if (rbLeft != null && !leftBounced)
            {
                rbLeft.linearVelocity = new Vector3(rbLeft.linearVelocity.x, bounceForce, rbLeft.linearVelocity.z);
                leftBounced = true;
            }

            if (rbRight != null && !rightBounced)
            {
                rbRight.linearVelocity = new Vector3(rbRight.linearVelocity.x, bounceForce, rbRight.linearVelocity.z);
                rightBounced = true;
            }

            if (rbSplitHalo != null && !haloBounced)
            {
                rbSplitHalo.linearVelocity = new Vector3(rbSplitHalo.linearVelocity.x, bounceForce, rbSplitHalo.linearVelocity.z);
                haloBounced = true;
            }
        }
    }
}
