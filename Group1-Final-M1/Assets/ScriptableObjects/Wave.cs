using UnityEngine;

[CreateAssetMenu(fileName = "NewWave", menuName = "EnemyWave")]
public class Wave : ScriptableObject
{
    [Header("Wave Settings")]
    public int waveNumber;
    public int totalEnemiesToSpawn; 
    public float spawnRate; 

    [Header("Enemy Type Frequencies")]
    [Range(0f, 1f)]
    public float normalEnemyWeight = 0.5f; 
    [Range(0f, 1f)]
    public float hardEnemyWeight = 0.25f; 
    [Range(0f, 1f)]
    public float eliteEnemyWeight = 0.25f;

    [Header("Enemy Prefabs")]
    public EnemyData normalEnemy;  
    public EnemyData hardEnemy;    
    public EnemyData eliteEnemy;   
    private void OnValidate()
    {
        float totalWeight = normalEnemyWeight + hardEnemyWeight + eliteEnemyWeight;
        if (totalWeight > 1f)
        {
            Debug.LogWarning("Total enemy weights exceed 1. Adjusting to maintain valid weights.");
            float excess = totalWeight - 1f;
            normalEnemyWeight -= excess * (normalEnemyWeight / totalWeight);
            hardEnemyWeight -= excess * (hardEnemyWeight / totalWeight);
            eliteEnemyWeight -= excess * (eliteEnemyWeight / totalWeight);
        }
    }
}