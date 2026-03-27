using UnityEngine;
using UnityEngine.AI;

public class WanderState : StateMachineBehaviour
{
    private NavMeshAgent navAgent;
    private EnemyController enemyController;

    public float wanderDistance = 0f;
    public float wanderRadius = 15f;

    private bool initialized = false;

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

        if (navAgent.hasPath)
        {
            Vector3 lookDirection = (navAgent.steeringTarget - navAgent.transform.position).normalized;
            Vector3 moveDirection = navAgent.transform.InverseTransformDirection(lookDirection);

            enemyController.lookDirection = lookDirection;
            enemyController.moveDirection = moveDirection;
        }
        else
        {
            SteeringUtility.Wander(navAgent, wanderDistance, wanderRadius);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        navAgent.ResetPath();
    }
}