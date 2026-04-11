using UnityEngine;

[ExecuteAlways]
public class CameraBillboard : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private bool keepVertical = false;
    [SerializeField] private bool invertFacing = false;

    private void LateUpdate()
    {
        UpdateBillboardRotation();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UpdateBillboardRotation();
        }
    }
#endif

    private void UpdateBillboardRotation()
    {
        Camera cameraToUse = ResolveCamera();
        if (cameraToUse == null)
        {
            return;
        }

        Transform cameraTransform = cameraToUse.transform;
        Vector3 forward = GetForward(cameraTransform);
        if (forward.sqrMagnitude < 0.0001f)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(forward, GetUp(cameraTransform));
    }

    private Camera ResolveCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        if (Camera.main != null)
        {
            return Camera.main;
        }

        return Camera.current;
    }

    private Vector3 GetForward(Transform cameraTransform)
    {
        Vector3 forward = cameraTransform.forward;

        if (keepVertical)
        {
            forward = Vector3.ProjectOnPlane(forward, Vector3.up);

            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector3.ProjectOnPlane(cameraTransform.position - transform.position, Vector3.up);
            }
        }

        forward.Normalize();
        return invertFacing ? -forward : forward;
    }

    private Vector3 GetUp(Transform cameraTransform)
    {
        return keepVertical ? Vector3.up : cameraTransform.up;
    }
}
