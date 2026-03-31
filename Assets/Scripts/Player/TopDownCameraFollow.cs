using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 11.77f, -9.34f);
    [SerializeField] private Vector3 eulerRotation = new Vector3(45f, 0f, 0f);
    [SerializeField] private float followSmoothTime = 0.12f;

    private Vector3 currentVelocity;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSmoothTime);
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
}
