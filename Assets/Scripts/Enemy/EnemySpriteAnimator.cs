using UnityEngine;

[DisallowMultipleComponent]
public class EnemySpriteAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private EnemyController enemyController;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite moveSprite;
    [SerializeField] private Sprite shootSprite;

    [Header("Tuning")]
    [SerializeField] private float moveThreshold = 0.05f;
    [SerializeField] private float shootFlashDuration = 0.06f;

    private Sprite initialSprite;
    private float shootFlashTimer;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (enemyController == null)
        {
            enemyController = GetComponentInParent<EnemyController>();
        }

        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite;
        }

        RefreshFromEnemyData();
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

    public void RefreshFromEnemyData()
    {
        if (enemyController == null || enemyController.enemyData == null)
        {
            return;
        }

        if (idleSprite == null && enemyController.enemyData.enemySprite != null)
        {
            idleSprite = enemyController.enemyData.enemySprite;
        }

        if (shootSprite == null && enemyController.enemyData.enemyShootSprite != null)
        {
            shootSprite = enemyController.enemyData.enemyShootSprite;
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
        if (enemyController == null)
        {
            return false;
        }

        return enemyController.moveDirection.sqrMagnitude > moveThreshold * moveThreshold;
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
