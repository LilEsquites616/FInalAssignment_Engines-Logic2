using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public static class SteeringUtility
{
    public static Vector3 Seek(Vector3 origin, Vector3 target)
    {
        Vector3 direction = target - origin;

        return direction;
    }

    public static Vector3 Seek(Transform origin, Transform target)
        => Seek(origin.position, target.position);

    public static void Seek(NavMeshAgent navAgent, Transform target)
    {
        navAgent.SetDestination(target.position);
    }

    public static Vector3 Flee(Vector3 origin, Vector3 target)
        => Seek(target, origin);

    public static Vector3 Flee(Transform origin, Transform target)
        => Seek(target.position, origin.position);

    public static void Flee(NavMeshAgent navAgent, Transform target, float safeRadius)
    {
        Vector3 direction = navAgent.transform.position - target.position;
        Vector3 safePoint = navAgent.transform.position + direction.normalized * safeRadius;

        if (NavMesh.SamplePosition(safePoint, out NavMeshHit hitInfo, safeRadius, NavMesh.AllAreas))
            navAgent.SetDestination(hitInfo.position);
    }

    public static Vector3 Wander(Transform origin, float wanderDistance ,float wanderRadius)
    {
        Vector3 circleCentre = origin.position + origin.forward * wanderDistance;
        Vector2 randomPoint = Random.insideUnitCircle.normalized * wanderRadius;
        Vector3 wanderPoint = circleCentre + new Vector3(randomPoint.x, origin.position.y, randomPoint.y);

        Vector3 direction = wanderPoint - origin.position;
        return direction;
    }

    public static void Wander(NavMeshAgent navAgent, float wanderDistance, float wanderRadius)
    {
        Vector3 circleCentre = navAgent.transform.position + navAgent.transform.forward * wanderDistance;
        Vector2 randomPoint = Random.insideUnitCircle.normalized * wanderRadius;
        Vector3 wanderPoint = circleCentre + new Vector3(randomPoint.x, navAgent.transform.position.y, randomPoint.y);

        if (NavMesh.SamplePosition(wanderPoint, out NavMeshHit hitInfo, wanderDistance + wanderRadius, NavMesh.AllAreas))
            navAgent.SetDestination(hitInfo.position);
    }
}