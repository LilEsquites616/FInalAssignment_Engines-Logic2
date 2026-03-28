using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f;
    public bool damagePlayer = true;
    public bool damageEnemies;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (damageEnemies)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(Mathf.RoundToInt(damage));
                Destroy(gameObject);
                return;
            }
        }

        if (damagePlayer)
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }
        }

        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
