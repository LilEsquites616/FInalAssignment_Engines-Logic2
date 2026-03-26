using UnityEngine;
using UnityEngine.AI;

public class FleeState : StateMachineBehaviour
{
    private EnemyController enemyController;
    private NavMeshAgent navAgent;

    private bool initialized = false;

    private readonly int safeParam = Animator.StringToHash("Safe");

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

        Vector3 currentPosition = navAgent.transform.position;
        Vector3 targetPosition = enemyController.Target.position;


        Vector3 fleeDirection = (currentPosition - targetPosition).normalized;

        float fleeDistance = enemyController.safeRadius;
        Vector3 fleeTarget = currentPosition + fleeDirection * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
        }

        Vector3 lookDirection = (navAgent.steeringTarget - currentPosition).normalized;
        Vector3 moveDirection = navAgent.transform.InverseTransformDirection(lookDirection);

        enemyController.lookDirection = lookDirection;
        enemyController.moveDirection = moveDirection;

        float distance = Vector3.Distance(currentPosition, targetPosition);
        if (distance > enemyController.safeRadius)
        {
            animator.SetTrigger(safeParam);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.ResetPath();
    }
}