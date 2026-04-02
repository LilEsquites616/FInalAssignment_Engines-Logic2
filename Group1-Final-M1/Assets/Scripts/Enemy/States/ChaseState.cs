using UnityEngine;
using UnityEngine.AI;

public class ChaseState : StateMachineBehaviour
{
    private EnemyController enemyController;
    private NavMeshAgent navAgent;

    private bool initialized = false;

    private readonly int attackParam = Animator.StringToHash("Attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (initialized) return;

        enemyController = animator.GetComponent<EnemyController>();
        navAgent = animator.GetComponent<NavMeshAgent>();

        initialized = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.IsInTransition(0)) return;

        if (enemyController.Target!=null)
        {
            navAgent.SetDestination(enemyController.Target.position);

            Vector3 lookDirection = (navAgent.steeringTarget - navAgent.transform.position).normalized;
            Vector3 moveDirection = navAgent.transform.InverseTransformDirection(lookDirection);

            enemyController.lookDirection = lookDirection;
            enemyController.moveDirection = moveDirection;
        
            if (Vector3.Distance(navAgent.transform.position, enemyController.Target.position) < enemyController.attackRadius)
                animator.SetTrigger(attackParam);
        }
            
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.ResetPath();
    }
}