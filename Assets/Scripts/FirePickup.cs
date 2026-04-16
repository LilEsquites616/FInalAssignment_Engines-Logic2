using UnityEngine;

public class FirePickup : MonoBehaviour
{
    [Header("Boost")]
    [SerializeField] private int projectileCount = 3;
    [SerializeField] private float duration = 10f;
    [SerializeField] private string promptLabel = "Fire x3";

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

        PlayerShooter playerShooter = other.GetComponentInParent<PlayerShooter>();

        if (playerShooter == null)
        {
            return;
        }

        collected = true;
        AudioManager.Instance?.PlayPickup();
        playerShooter.ApplyTemporaryMultiShot(projectileCount, duration);
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
