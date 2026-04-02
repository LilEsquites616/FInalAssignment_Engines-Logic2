using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public AdManager adManager;
    public TextMeshProUGUI healthText;
    private void Awake()
    {
        currentHealth = maxHealth;
        healthText.text = $"{maxHealth}";
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthText.text = $"{currentHealth}";
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Player died");
        GameOverHandler.Instance.TriggerGameOver(false);
        adManager.LoadAd("Interstitial");
        adManager.ShowAd("Interstitial");
    }
}