using System;
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyDropper : MonoBehaviour
{
    [Serializable]
    public class DropOption
    {
        [SerializeField] private GameObject powerUpPrefab;
        [SerializeField, Range(0f, 100f)] private float dropChance = 10f;

        public GameObject PowerUpPrefab => powerUpPrefab;
        public float DropChance => dropChance;
    }

    [Header("Drop Settings")]
    [SerializeField] private DropOption[] dropOptions;
    [SerializeField] private Vector3 dropOffset = new Vector3(0f, 0.5f, 0f);
    [SerializeField] private bool randomizeYaw = true;

    private EnemyHealth enemyHealth;
    private bool hasDropped;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    private void OnEnable()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        enemyHealth.OnDeath += HandleDeath;
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnDeath -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        if (hasDropped)
        {
            return;
        }

        hasDropped = true;
        TryDropPowerUp();
    }

    private void TryDropPowerUp()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        float accumulatedChance = 0f;

        foreach (DropOption dropOption in dropOptions)
        {
            if (dropOption == null || dropOption.PowerUpPrefab == null)
            {
                continue;
            }

            accumulatedChance += Mathf.Clamp(dropOption.DropChance, 0f, 100f);

            if (roll > accumulatedChance)
            {
                continue;
            }

            SpawnDrop(dropOption.PowerUpPrefab);
            return;
        }
    }

    private void SpawnDrop(GameObject powerUpPrefab)
    {
        Vector3 spawnPosition = transform.position + dropOffset;
        Quaternion spawnRotation = powerUpPrefab.transform.rotation;

        if (randomizeYaw)
        {
            spawnRotation *= Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
        }

        Instantiate(powerUpPrefab, spawnPosition, spawnRotation);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (dropOptions == null)
        {
            return;
        }

        float totalChance = 0f;

        foreach (DropOption dropOption in dropOptions)
        {
            if (dropOption == null)
            {
                continue;
            }

            totalChance += Mathf.Clamp(dropOption.DropChance, 0f, 100f);
        }

        if (totalChance > 100f)
        {
            Debug.LogWarning($"EnemyDropper on {name} has a total drop chance above 100%. The last entries in the array may never be selected.", this);
        }
    }
#endif
}
