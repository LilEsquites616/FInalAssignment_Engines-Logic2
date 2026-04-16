
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");
    private static readonly int ShootParam = Animator.StringToHash("Shoot");

    public EnemyData enemyData;
    [SerializeField] private Rigidbody body;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteFrameAnimator spriteAnimator;
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
    private float distance;
    private readonly int hasTargetParam = Animator.StringToHash("Player Close");
    [SerializeField] private AudioSource detectSfx;
    [SerializeField] private  AudioSource unDetectSfx;
    private bool hasVisualMoveParam;
    private bool hasVisualShootParam;

    private void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteAnimator == null) spriteAnimator = GetComponentInChildren<SpriteFrameAnimator>();
        if (navAgent == null) navAgent = GetComponent<NavMeshAgent>();
        if (GameObject.FindWithTag("Player")!=null)
            Target = GameObject.FindWithTag("Player").transform;

        CacheAnimatorParameters();
    }
    public void StatPass()
    {
        navAgent.speed = enemyData.speed;
        turnSpeed = enemyData.turnSpeed;
        attackRadius = enemyData.attackRadius;
        safeRadius = enemyData.safeRadius;

        if (animator != null && enemyData.enemyAnimator != null)
        {
            animator.runtimeAnimatorController = enemyData.enemyAnimator;
            CacheAnimatorParameters();
        }

        spriteAnimator?.RefreshFromEnemyData();
    }

    public void PlayShootFlash()
    {
        if (animator != null && hasVisualShootParam)
        {
            animator.ResetTrigger(ShootParam);
            animator.SetTrigger(ShootParam);
        }

        spriteAnimator?.PlayShootFlash();
    }

    private void Update()
    {
        if (Target!=null)
            distance = Vector3.Distance(transform.position, Target.position);
        bool isTargetClose = distance <= safeRadius;
        animator.SetBool(hasTargetParam, isTargetClose);

        if (animator != null && hasVisualMoveParam)
        {
            bool isMoving = moveDirection.sqrMagnitude > 0.0025f;
            animator.SetBool(IsMovingParam, isMoving);
        }
        
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

    private void CacheAnimatorParameters()
    {
        hasVisualMoveParam = HasParameter("IsMoving", AnimatorControllerParameterType.Bool);
        hasVisualShootParam = HasParameter("Shoot", AnimatorControllerParameterType.Trigger);
    }

    private bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }
}
