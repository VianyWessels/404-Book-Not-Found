using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public AudioManager audioManager;
    public bool isDead;
    public float invincibilityTime;

    public Canvas damageFlashCanvas;
    public float damageFlashDuration;

    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
        deathScreen.enabled = false;
        if (damageFlashCanvas != null)
            damageFlashCanvas.enabled = false;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHearts();

        if (audioManager != null)
            audioManager.PlayDamage();

        if (damageFlashCanvas != null)
        {
            if (flashCoroutine != null)
                StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityFrames());
    }

    private IEnumerator DamageFlash()
    {
        Image flashImage = damageFlashCanvas.GetComponentInChildren<Image>();
        CanvasGroup canvasGroup = damageFlashCanvas.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = damageFlashCanvas.gameObject.AddComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        damageFlashCanvas.enabled = true;

        float timer = 0f;
        while (timer < damageFlashDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / damageFlashDuration);
            yield return null;
        }

        damageFlashCanvas.enabled = false;
    }

    private IEnumerator InvincibilityFrames()
    {
        playerDamage.enabled = false;
        yield return new WaitForSeconds(invincibilityTime);
        playerDamage.enabled = true;
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator != null) animator.SetTrigger("Die");
        if (playerMovement != null) playerMovement.enabled = false;
        if (playerDamage != null) playerDamage.enabled = false;
        StartCoroutine(ShowDeathScreenAfterDelay(1.5f));
    }

    private IEnumerator ShowDeathScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (deathScreen != null) deathScreen.enabled = true;
        Time.timeScale = 0f;
    }

    private void UpdateHearts()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
        }
    }

    public int GetCurrentHealth() => currentHealth;

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        UpdateHearts();
        StopAllCoroutines();
        enabled = true;
        if (playerDamage != null) playerDamage.enabled = true;
    }
}