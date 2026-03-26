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
        StatPass();
    }
    public void StatPass()
    {
        health = enemyData.health;
    }
}
