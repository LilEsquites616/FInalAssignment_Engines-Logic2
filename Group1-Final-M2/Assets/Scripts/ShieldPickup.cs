using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField] private float incomingDamageMultiplier = 0.5f;
    [SerializeField] private float duration = 10f;
    [SerializeField] private string promptLabel = "Shield";

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

        PlayerHealth playerHealth = other.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            return;
        }

        collected = true;
        playerHealth.ApplyTemporaryShield(incomingDamageMultiplier, duration);
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
