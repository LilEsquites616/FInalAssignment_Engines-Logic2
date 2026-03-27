using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public EnemyData enemyData;  
    [SerializeField] private int health;
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    void Start()
    {
        health = enemyData.health;
    }
    public void StatPass()
    {
        health = enemyData.health;
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die(true);
        }
    }
    private void Die(bool giveScore)
    {
        Destroy(gameObject);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}
