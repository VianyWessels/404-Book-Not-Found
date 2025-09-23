using UnityEngine;
using System.Collections;

public class CrucifixDamage : MonoBehaviour
{
    public int damage;
    public PlayerHealth playerhealth;
    public Collider rangeCollider;
    public LayerMask playerLayer;

    private void Start()
    {
        if (rangeCollider == null)
        {
            return;
        }

        StartCoroutine(DamagePlayerOverTime());
    }

    private IEnumerator DamagePlayerOverTime()
    {
        while (true)
        {
            Vector3 center = rangeCollider.bounds.center;
            float radius = rangeCollider.bounds.extents.x;

            Collider[] hits = Physics.OverlapSphere(center, radius, playerLayer);

            foreach (Collider hit in hits)
            {
                playerhealth.TakeDamage(damage);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}
