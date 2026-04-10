using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing = 5f;
    public bool stopFollowing = false;
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.1f;
    public float shakeFrequency = 2f;

    private Vector3 offset;
    private Vector3 shakeOffset;
    private Vector3 originalPosition;

    void Start()
    {
        offset = transform.position - target.position;
        originalPosition = transform.position;
    }

    void FixedUpdate()
    {
        if (shakeDuration > 0)
        {
            ApplyShake();
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        if (!stopFollowing)
        {
            Vector3 targetCamPos = target.position + offset + shakeOffset;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
        }
    }

    private void ApplyShake()
    {

        shakeOffset = new Vector3(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude), 0);

        shakeDuration -= Time.deltaTime * shakeFrequency;

        if (shakeDuration <= 0)
        {
            shakeDuration = 0;
            shakeOffset = Vector3.zero; 
        }
    }

    public void StopFollow()
    {
        stopFollowing = true;
    }

    public void Follow()
    {
        stopFollowing = false;
    }

    public void TriggerShake(float duration, float magnitude, float frequency)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
        shakeFrequency = frequency;
    }
}
