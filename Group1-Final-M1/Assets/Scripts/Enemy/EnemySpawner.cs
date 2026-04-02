using UnityEngine;
using System.Collections;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using System.IO;

public class EnemySpawner : MonoBehaviour
{
    [Header("Wave Data")]
    public Wave[] waveData;
    public int currentWaveIndex = 0;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    [Header("UI Elements")]
    public TextMeshProUGUI waveCount;
    public TextMeshProUGUI waveDescription;

    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    private float[] spawnWeights;
    private int enemiesDefeated = 0;
    private int totalEnemiesInWave = 0;

    [Header("Game Management")]
    public GameObject gameOverHandler;

    void Start()
    {
        StartWave(waveData[currentWaveIndex]);
    }

    public void StartWave(Wave wave)
    {
        spawnWeights = new float[] { wave.normalEnemyWeight, wave.hardEnemyWeight, wave.eliteEnemyWeight };
        totalEnemiesInWave = wave.totalEnemiesToSpawn;
        enemiesDefeated = 0;

        waveCount.text = "Wave: " + wave.waveNumber;
        waveDescription.text = "Kill " + totalEnemiesInWave + " enemies";
        StartCoroutine(SpawnEnemiesForWave(wave));
    }

    private IEnumerator SpawnEnemiesForWave(Wave wave)
    {
        int enemiesLeftToSpawn = wave.totalEnemiesToSpawn;

        while (enemiesLeftToSpawn > 0)
        {
            float totalWeight = spawnWeights.Sum();
            float randomValue = Random.Range(0f, totalWeight);

            EnemyData enemyData = GetRandomEnemyData(wave, randomValue);

            GameObject enemy = Instantiate(enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            EnemyController controller = enemy.GetComponent<EnemyController>();

            health.enemyData = enemyData;
            controller.enemyData = enemyData;

            health.StatPass();
            controller.StatPass();

            health.OnDeath += OnEnemyDefeated;

            enemiesLeftToSpawn--;

            yield return new WaitForSeconds(wave.spawnRate);
        }
    }

    private EnemyData GetRandomEnemyData(Wave wave, float randomValue)
    {
        if (randomValue < wave.normalEnemyWeight)
        {
            return wave.normalEnemy;
        }
        else if (randomValue < wave.normalEnemyWeight + wave.hardEnemyWeight)
        {
            return wave.hardEnemy;
        }
        else
        {
            return wave.eliteEnemy;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return Vector3.zero;
        }

        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex].position;
    }


    private void OnEnemyDefeated()
    {
        enemiesDefeated++;
        int remainingEnemies = totalEnemiesInWave - enemiesDefeated;
        waveDescription.text = "Kill " + remainingEnemies + " enemies";
        if (enemiesDefeated >= totalEnemiesInWave)
        {
            if (currentWaveIndex+1 < waveData.Length)
            {
                StartNextWave();
            }
            else
            {
                waveDescription.text = "All waves completed!";
                gameOverHandler.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void StartNextWave()
    {
        currentWaveIndex++;

        if (currentWaveIndex < waveData.Length)
        {
            StartWave(waveData[currentWaveIndex]);
        }
    }
}
