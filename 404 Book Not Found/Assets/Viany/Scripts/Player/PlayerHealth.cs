using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public Image[] heartIcons;
    public Color fullHeart;
    public Color emptyHeart;
    public Animator animator;

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
        {
            Die();
        }
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].color = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        Time.timeScale = 0f;
    }

    public int GetCurrentHealth() => currentHealth;
}
