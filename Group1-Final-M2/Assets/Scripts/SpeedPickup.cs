using UnityEngine;

public class SpeedPickup : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField] private float speedMultiplier = 2f;
    [SerializeField] private float duration = 10f;
    [SerializeField] private string promptLabel = "Speedx2";

    [Header("Pickup")]
    [SerializeField] private Collider pickupCollider;
    [SerializeField] private GameObject pickupVisual;
    [SerializeField] private bool destroyOnPickup = true;

    private bool collected;

    private void Awake()
    {
        if (pickupCollider == null)
        {
            pickupCollider = GetComponent<Collider>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        PlayerController playerController = other.GetComponentInParent<PlayerController>();

        if (playerController == null)
        {
            return;
        }

        collected = true;
        playerController.ApplyTemporarySpeedBoost(speedMultiplier, duration);
        PickupPromptUI.Instance.ShowTimedPrompt(promptLabel, duration);

        if (pickupCollider != null)
        {
            pickupCollider.enabled = false;
        }

        if (pickupVisual != null)
        {
            pickupVisual.SetActive(false);
        }

        if (destroyOnPickup)
        {
            Destroy(gameObject);
            return;
        }

        gameObject.SetActive(false);
    }
}
