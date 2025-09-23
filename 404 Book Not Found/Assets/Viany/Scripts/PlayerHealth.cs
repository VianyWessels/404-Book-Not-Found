using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public Image[] heartIcons;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHearts();

        if (currentHealth <= 0)
            Die();
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null)
                heartIcons[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void Die()
    {
        Debug.Log("Player died");

        // Freeze the game
        Time.timeScale = 0f;

        // Optional: show a "Game Over" UI here
    }

    public int GetCurrentHealth() => currentHealth;
}
