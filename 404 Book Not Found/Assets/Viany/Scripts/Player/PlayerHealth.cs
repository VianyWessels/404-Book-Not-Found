using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth;
    private int currentHealth;

    public Image[] heartIcons;
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Animator animator;

    public Canvas deathScreen;
    public PlayerMovement playerMovement;
    public PlayerDamage playerDamage;

    private bool isDead;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
            deathScreen.enabled = false;
    }

    public bool IsReady()
    {
        return animator != null;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

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
            heartIcons[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Die");

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerDamage != null)
            playerDamage.enabled = false;

        StartCoroutine(ShowDeathScreenAfterDelay(1.5f));
    }

    private System.Collections.IEnumerator ShowDeathScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (deathScreen != null)
            deathScreen.enabled = true;

        Time.timeScale = 0f;
    }

    public int GetCurrentHealth() => currentHealth;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

}
