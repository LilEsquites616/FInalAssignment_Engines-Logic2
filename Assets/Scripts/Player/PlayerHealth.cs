using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float baseMaxHealth = 100f;
    public float maxHealth;
    public float currentHealth;
    public AdManager adManager;
    public TextMeshProUGUI healthText;

    private void Awake()
    {
        maxHealth = baseMaxHealth;
        if (ModsManager.Instance != null && ModsManager.Instance.hpActive)
        {
            maxHealth += 50f;

        }

        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        if (GameOverHandler.Instance != null)
        {
            GameOverHandler.Instance.TriggerGameOver(false);
        }

        if (adManager != null)
        {
            adManager.LoadAd("Interstitial");
            adManager.ShowAd("Interstitial");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}";
    }
}