using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public AdManager adManager;
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel; 
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
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        adManager.LoadAd("Interstitial");
        adManager.ShowAd("Interstitial");
    }
}