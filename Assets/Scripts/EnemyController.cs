
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public EnemyData enemyData;
    [SerializeField] private Rigidbody body;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navAgent;
    public Transform Target;
    public float turnSpeed = 180f;
    public float attackRadius = 8f;
    public float safeRadius = 5f;
    public float speed = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    [HideInInspector] public Vector3 moveDirection;
    [HideInInspector] public Vector3 lookDirection;
    private Vector3 startPosition;
    private readonly int hasTargetParam = Animator.StringToHash("Player Close");
    [SerializeField] private AudioSource detectSfx;
    [SerializeField] private  AudioSource unDetectSfx;

    private void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (enemyData!=null)
        {
            navAgent.speed = enemyData.speed;
            turnSpeed = enemyData.turnSpeed;
            attackRadius = enemyData.attackRadius;
            safeRadius = enemyData.safeRadius;
        }
        Target = GameObject.FindWithTag("Player").transform;
        StatPass();

    }
    public void StatPass()
    {
        navAgent.speed = enemyData.speed;
        turnSpeed = enemyData.turnSpeed;
        attackRadius = enemyData.attackRadius;
        safeRadius = enemyData.safeRadius;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, Target.position);
        bool isTargetClose = distance <= safeRadius;
        animator.SetBool(hasTargetParam, isTargetClose);
        
        if (animator != null)
        {
            if (lookDirection != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                desiredRotation,
                turnSpeed * Time.deltaTime);
            }
        }
    }
}