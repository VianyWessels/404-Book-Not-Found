using UnityEngine;
using System.Collections;

public class CrucifixDamage : MonoBehaviour
{
    public int damage = 1;       // Damage applied per second
    public Collider rangeCollider;         // Assign the range object’s collider here
    public LayerMask playerLayer;         // Player layer for detection

    private void Start()
    {
        if (rangeCollider == null)
        {
            Debug.LogWarning("Range collider not assigned!");
            return;
        }

        StartCoroutine(DamagePlayerOverTime());
    }

    private IEnumerator DamagePlayerOverTime()
    {
        while (true)
        {
            if (rangeCollider != null)
            {
                // Determine center and radius from collider
                Vector3 center = rangeCollider.bounds.center;
                float radius = rangeCollider.bounds.extents.x; // Assumes roughly spherical/circular

                Collider[] hits = Physics.OverlapSphere(center, radius, playerLayer);

                foreach (Collider hit in hits)
                {
                    PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f); // Damage every 1 second
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (rangeCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(rangeCollider.bounds.center, rangeCollider.bounds.extents.x);
        }
    }
}
