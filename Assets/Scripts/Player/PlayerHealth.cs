using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public AdManager adManager;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    private void Die()
    {
        Debug.Log("Player died");
        Destroy(gameObject);
        adManager.LoadAd("Interstitial");
        adManager.ShowAd("Interstitial");
    }
}