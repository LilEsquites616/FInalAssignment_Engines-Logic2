using UnityEngine;

public class CameraBillboard : MonoBehaviour
{
    public enum BillboardMode
    {
        Full,
        YAxisOnly
    }

    [Header("References")]
    [SerializeField] private Camera targetCamera;

    [Header("Billboard")]
    [SerializeField] private BillboardMode billboardMode = BillboardMode.Full;
    [SerializeField] private bool useMainCameraFallback = true;
    [SerializeField] private Vector3 rotationOffset;

    private void LateUpdate()
    {
        Camera cameraToUse = GetCamera();

        if (cameraToUse == null)
        {
            return;
        }

        Vector3 directionToCamera = cameraToUse.transform.position - transform.position;

        if (directionToCamera.sqrMagnitude < 0.0001f)
        {
            return;
        }

        Quaternion lookRotation;

        if (billboardMode == BillboardMode.YAxisOnly)
        {
            directionToCamera.y = 0f;

            if (directionToCamera.sqrMagnitude < 0.0001f)
            {
                return;
            }

            lookRotation = Quaternion.LookRotation(-directionToCamera.normalized, Vector3.up);
        }
        else
        {
            lookRotation = Quaternion.LookRotation(-directionToCamera.normalized, cameraToUse.transform.up);
        }

        transform.rotation = lookRotation * Quaternion.Euler(rotationOffset);
    }

    private Camera GetCamera()
    {
        if (targetCamera != null)
        {
            return targetCamera;
        }

        if (useMainCameraFallback)
        {
            return Camera.main;
        }

        return null;
    }
}
