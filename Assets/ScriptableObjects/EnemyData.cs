using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Header("Stats")]
    public int health;               
    public int damage;               
    public float turnSpeed = 180f;
    public float attackRadius = 8f;
    public float safeRadius = 5f;
    public float speed = 1f;

    [Header("Attack Settings")]
    public float attackSpeed = 1f;
    public float bulletSpeed = 20f;
    public bool canShoot = true;
    public int scoreValue = 100; 

    [Header("Visual")]
    public RuntimeAnimatorController enemyAnimator;
    public Sprite enemySprite;
    public Sprite[] enemyMoveFrames;
    public Sprite enemyShootSprite;
}
