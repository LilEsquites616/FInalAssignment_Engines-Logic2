using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody playerBody;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite moveSprite;
    [SerializeField] private Sprite shootSprite;

    [Header("Tuning")]
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float shootFlashDuration = 0.06f;

    private Sprite initialSprite;
    private float shootFlashTimer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerBody == null)
        {
            playerBody = GetComponentInParent<Rigidbody>();
        }

        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite;
        }
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (shootFlashTimer > 0f)
        {
            shootFlashTimer -= Time.deltaTime;
            SetSprite(shootSprite != null ? shootSprite : GetBaseSprite());
            return;
        }

        SetSprite(GetBaseSprite());
    }

    public void PlayShootFlash()
    {
        if (shootSprite == null)
        {
            return;
        }

        shootFlashTimer = shootFlashDuration;
        SetSprite(shootSprite);
    }

    private Sprite GetBaseSprite()
    {
        if (IsMoving() && moveSprite != null)
        {
            return moveSprite;
        }

        if (idleSprite != null)
        {
            return idleSprite;
        }

        return initialSprite;
    }

    private bool IsMoving()
    {
        if (playerBody == null)
        {
            return false;
        }

        Vector3 horizontalVelocity = new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z);
        return horizontalVelocity.sqrMagnitude > moveThreshold * moveThreshold;
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite == null || spriteRenderer.sprite == sprite)
        {
            return;
        }

        spriteRenderer.sprite = sprite;
    }
}
