using UnityEngine;
using System.Collections.Generic;

public class CrucifixDamage : MonoBehaviour
{
    public int damage;
    public float damageInterval;
    public LayerMask playerLayer;

    private Dictionary<PlayerHealth, float> playersInTrigger = new();

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null && !playersInTrigger.ContainsKey(player))
            {
                playersInTrigger[player] = 0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null && playersInTrigger.ContainsKey(player))
        {
            playersInTrigger.Remove(player);
        }
    }

    private void Update()
    {
        if (playersInTrigger.Count == 0) return;

        List<PlayerHealth> keys = new(playersInTrigger.Keys);
        foreach (var player in keys)
        {
            playersInTrigger[player] -= Time.deltaTime;
            if (playersInTrigger[player] <= 0f)
            {
                player.TakeDamage(damage);
                playersInTrigger[player] = damageInterval;
            }
        }
    }
}
