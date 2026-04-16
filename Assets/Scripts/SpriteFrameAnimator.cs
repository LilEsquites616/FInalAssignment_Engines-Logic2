using UnityEngine;

public class SpriteFrameAnimator : MonoBehaviour
{
    public enum ActorType
    {
        Player,
        Enemy
    }

    [Header("Setup")]
    [SerializeField] private ActorType actorType = ActorType.Player;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Player References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerBody;

    [Header("Enemy References")]
    [SerializeField] private EnemyController enemyController;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite[] moveFrames;
    [SerializeField] private Sprite shootSprite;

    [Header("Animation")]
    [SerializeField] private float moveFramesPerSecond = 10f;
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float shootFrameDuration = 0.06f;

    [Header("Facing")]
    [SerializeField] private bool flipSpriteWithAim = true;
    [SerializeField] private bool invertHorizontalFacing = false;
    [SerializeField] private bool useFacingPositions = false;
    [SerializeField] private Vector3 faceRightLocalPosition;
    [SerializeField] private Vector3 faceLeftLocalPosition;

    private Sprite initialSprite;
    private float shootTimer;
    private float moveTimer;
    private int moveFrameIndex;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (actorType == ActorType.Player)
        {
            if (playerController == null)
            {
                playerController = GetComponentInParent<PlayerController>();
            }

            if (playerBody == null)
            {
                playerBody = GetComponentInParent<Rigidbody>();
            }
        }
        else
        {
            if (enemyController == null)
            {
                enemyController = GetComponentInParent<EnemyController>();
            }
        }

        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite;
        }

        InitializeFacingPositions();
        RefreshFromEnemyData();
        SetSprite(GetCurrentBaseSprite());
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        UpdateFacing();

        if (shootTimer > 0f)
        {
            shootTimer -= Time.deltaTime;
            SetSprite(shootSprite != null ? shootSprite : GetCurrentBaseSprite());
            return;
        }

        if (IsMoving() && moveFrames != null && moveFrames.Length > 0)
        {
            moveTimer += Time.deltaTime;
            float frameDuration = 1f / Mathf.Max(1f, moveFramesPerSecond);

            while (moveTimer >= frameDuration)
            {
                moveTimer -= frameDuration;
                moveFrameIndex = (moveFrameIndex + 1) % moveFrames.Length;
            }

            SetSprite(moveFrames[moveFrameIndex]);
            return;
        }

        moveTimer = 0f;
        moveFrameIndex = 0;
        SetSprite(GetCurrentBaseSprite());
    }

    public void PlayShootFlash()
    {
        shootTimer = shootFrameDuration;
        SetSprite(shootSprite != null ? shootSprite : GetCurrentBaseSprite());
    }

    public void RefreshFromEnemyData()
    {
        if (actorType != ActorType.Enemy || enemyController == null || enemyController.enemyData == null)
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
    }

    private bool IsMoving()
    {
        if (actorType == ActorType.Player)
        {
            if (playerBody == null)
            {
                return false;
            }

            Vector3 horizontalVelocity = new Vector3(playerBody.linearVelocity.x, 0f, playerBody.linearVelocity.z);
            return horizontalVelocity.sqrMagnitude > moveThreshold * moveThreshold;
        }

        if (enemyController == null)
        {
            return false;
        }

        return enemyController.moveDirection.sqrMagnitude > moveThreshold * moveThreshold;
    }

    private void UpdateFacing()
    {
        Vector3 facingDirection = GetFacingDirection();

        if (facingDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        bool facingRight = facingDirection.x >= 0f;

        if (invertHorizontalFacing)
        {
            facingRight = !facingRight;
        }

        if (flipSpriteWithAim)
        {
            spriteRenderer.flipX = !facingRight;
        }

        if (useFacingPositions)
        {
            transform.localPosition = facingRight ? faceRightLocalPosition : faceLeftLocalPosition;
        }
    }

    private Vector3 GetFacingDirection()
    {
        if (actorType == ActorType.Player)
        {
            return playerController != null ? playerController.GetAimDirection() : Vector3.zero;
        }

        return enemyController != null ? enemyController.lookDirection : Vector3.zero;
    }

    private Sprite GetCurrentBaseSprite()
    {
        if (idleSprite != null)
        {
            return idleSprite;
        }

        return initialSprite;
    }

    private void InitializeFacingPositions()
    {
        if (faceRightLocalPosition == Vector3.zero && faceLeftLocalPosition == Vector3.zero)
        {
            faceRightLocalPosition = transform.localPosition;
            faceLeftLocalPosition = new Vector3(
                -transform.localPosition.x,
                transform.localPosition.y,
                transform.localPosition.z);
        }
    }

    private void SetSprite(Sprite sprite)
    {
        if (sprite == null || spriteRenderer == null || spriteRenderer.sprite == sprite)
        {
            return;
        }

        spriteRenderer.sprite = sprite;
    }
}
