using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Header("Stats")]
    public int health;               
    public int damage;               
    public float speed;

    [Header("Attack Settings")]
    public float attackSpeed;
    public float bulletSpeed;
    public bool canShoot;
    public int scoreValue; 

    [Header("Visual")]
    public RuntimeAnimatorController enemyAnimator;
    public Sprite enemySprite;
}
