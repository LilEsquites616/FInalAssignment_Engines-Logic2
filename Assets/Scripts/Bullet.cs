using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyController>() != null)
        {
            return;
        }

        PlayerHealth target = other.GetComponent<PlayerHealth>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}