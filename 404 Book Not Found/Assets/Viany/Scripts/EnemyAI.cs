using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    public float moveSpeed;
    public float attackRange;
    public int attackDamage;
    public float attackCooldown;
    public float bobAmplitude;
    public float bobFrequency;
    public int maxHealth;
    public float detectionRange;

    public float fallSpeedMultiplier;
    public float minFallTime;
    public float splitForce;
    public float bounceForce;
    public GameObject leftHalf;
    public GameObject rightHalf;
    public PlayerHealth playerHealth;

    [SerializeField] private Transform player;
    private float nextAttackTime;
    private int currentHealth;
    [SerializeField] private Rigidbody rb;
    private bool isDead;
    [SerializeField] private Collider mainCollider;
    [SerializeField] private Renderer enemyRenderer;
    private bool leftBounced;
    private bool rightBounced;

    void Start()
    {
        currentHealth = maxHealth;
        leftHalf.SetActive(false);
        rightHalf.SetActive(false);
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            IdleBehavior();
        }
        else
        {
            MoveAndAttackPlayer(distance);
        }
    }

    private void IdleBehavior()
    {
        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);
        transform.Rotate(Vector3.up * Time.deltaTime * 20f);
    }

    private void MoveAndAttackPlayer(float distance)
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        float bob = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position += new Vector3(0f, bob * Time.deltaTime, 0f);

        transform.LookAt(player);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void AttackPlayer()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        enemyRenderer.enabled = false;
        mainCollider.enabled = false;

        leftHalf.SetActive(true);
        Rigidbody rbLeft = leftHalf.GetComponent<Rigidbody>();
        if (rbLeft != null)
        {
            rbLeft.isKinematic = false;
            rbLeft.linearVelocity = (Vector3.left + Vector3.up) * splitForce;
            rbLeft.angularVelocity = new Vector3(Random.value, Random.value, Random.value);
        }

        rightHalf.SetActive(true);
        Rigidbody rbRight = rightHalf.GetComponent<Rigidbody>();
        if (rbRight != null)
        {
            rbRight.isKinematic = false;
            rbRight.linearVelocity = (Vector3.right + Vector3.up) * splitForce;
            rbRight.angularVelocity = new Vector3(Random.value, Random.value, Random.value);
        }

        rb.useGravity = false;
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
            if (!leftBounced)
            {
                Rigidbody rbLeft = leftHalf.GetComponent<Rigidbody>();
                if (rbLeft != null)
                    rbLeft.linearVelocity = new Vector3(rbLeft.linearVelocity.x, bounceForce, rbLeft.linearVelocity.z);
                leftBounced = true;
            }

            if (!rightBounced)
            {
                Rigidbody rbRight = rightHalf.GetComponent<Rigidbody>();
                if (rbRight != null)
                    rbRight.linearVelocity = new Vector3(rbRight.linearVelocity.x, bounceForce, rbRight.linearVelocity.z);
                rightBounced = true;
            }
        }
    }
}