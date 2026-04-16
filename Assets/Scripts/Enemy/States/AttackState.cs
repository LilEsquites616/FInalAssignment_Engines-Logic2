using UnityEngine;
using UnityEngine.AI;

public class AttackState : StateMachineBehaviour
{
    private EnemyController enemyController;
    private NavMeshAgent navAgent;
    private float attackRadius;
    private bool initialized;
    private readonly int chaseParam = Animator.StringToHash("Chase");

    private float shootCooldown = 1f;
    private float lastShootTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (initialized) return;

        enemyController = animator.GetComponent<EnemyController>();
        navAgent = animator.GetComponent<NavMeshAgent>();

        attackRadius = enemyController.attackRadius;

        initialized = true;
        shootCooldown = enemyController.enemyData.attackSpeed;
        lastShootTime = -shootCooldown;

    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(0)) return;
        if (enemyController.Target!=null)
        {
            Vector3 lookDirection = (enemyController.Target.position - navAgent.transform.position).normalized;
            Vector3 moveDirection = (navAgent.steeringTarget - navAgent.transform.position).normalized;

            enemyController.lookDirection = lookDirection;
            enemyController.moveDirection = navAgent.transform.InverseTransformDirection(moveDirection);

            float distanceToTarget = Vector3.Distance(navAgent.transform.position, enemyController.Target.position);

            if (distanceToTarget > attackRadius)
            {
                animator.SetTrigger(chaseParam);
            }
            else
            {
                if (!navAgent.hasPath)
                    navAgent.SetDestination(GetNewAttackPosition());

                TryShootAtTarget();
            }
        }
    }

    private void TryShootAtTarget()
    {
        if (Time.time - lastShootTime >= shootCooldown)
        {
            ShootBullet();
            lastShootTime = Time.time;
        }
    }

    private void ShootBullet()
    {
        if (enemyController.bulletPrefab == null || enemyController.firePoint == null) return;

        GameObject bullet = Instantiate(enemyController.bulletPrefab, enemyController.firePoint.position, Quaternion.LookRotation(enemyController.Target.position - enemyController.firePoint.position));
        
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        Bullet bulletComponent = bullet.GetComponent<Bullet>();

        if (bulletComponent != null)
        {
            bulletComponent.damage = enemyController.enemyData.damage;
            bulletComponent.damagePlayer = true;
            bulletComponent.damageEnemies = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = (enemyController.Target.position - enemyController.firePoint.position).normalized * enemyController.bulletSpeed;
        }

        enemyController.PlayShootFlash();
        AudioManager.Instance?.PlayEnemyShot();
    }

    private Vector3 GetNewAttackPosition()
    {
        Vector3 randomPosition = enemyController.Target.position + Random.insideUnitSphere * attackRadius;

        if (NavMesh.SamplePosition(randomPosition, out NavMeshHit hitInfo, attackRadius, NavMesh.AllAreas))
            return hitInfo.position;
        else
            return navAgent.transform.position;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.ResetPath();
    }
}
