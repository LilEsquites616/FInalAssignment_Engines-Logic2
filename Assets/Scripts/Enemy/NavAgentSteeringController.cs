using UnityEngine;
using UnityEngine.AI;

public class NavAgentSteeringController : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Transform target;
    public SteeringType steeringType = SteeringType.Wander;
    [Header("Flee Settings")]
    public float safeRadius = 5f;
    [Header("Wander Settings")]
    public bool doSeekOnTriggerEnter;
    public bool doFleeOnTriggerEnter;
    public float wanderDistance = 5f;
    public float wanderRadius = 5f;
    [Header("Update Settings")]
    public float updateDelayInSeconds = 0.5f;
    private float timeSinceLastUpdate = 0f;
    [SerializeField] private AudioSource detectSfx;
    [SerializeField] private  AudioSource unDetectSfx;

    void Update()
    {
        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate > updateDelayInSeconds)
        {
            timeSinceLastUpdate = 0f;
            switch (steeringType)
            {
                case SteeringType.Seek:
                    SteeringUtility.Seek(navAgent, target.transform);
                    break;
                case SteeringType.Flee:
                    SteeringUtility.Flee(navAgent, target.transform, safeRadius);
                    break;
                case SteeringType.Wander:
                    SteeringUtility.Wander(navAgent, wanderDistance, wanderRadius);
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == target && steeringType == SteeringType.Wander)
        {
            if (doFleeOnTriggerEnter)
            {
                target = other.transform;
                steeringType = SteeringType.Flee;
            }
            if (doSeekOnTriggerEnter)
            {
                steeringType = SteeringType.Seek;
                detectSfx.Play();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
       if (other.transform == target && steeringType != SteeringType.SeekWaypoint)
        {
            steeringType = SteeringType.Wander;
            unDetectSfx.Play();
        }
    }
    public void ReassignTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
