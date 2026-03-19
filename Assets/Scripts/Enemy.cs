using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyData enemyData;  
    [SerializeField] private int health;
    [SerializeField] private int damage;
    public delegate void DeathHandler();
    public event DeathHandler OnDeath;

    void Start()
    {
        health = enemyData.health;
        damage = enemyData.damage;  
    }
}
