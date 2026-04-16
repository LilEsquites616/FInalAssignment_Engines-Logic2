using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSpriteAnimator : MonoBehaviour
{
    private static readonly int ShootParam = Animator.StringToHash("Shoot");
    private static readonly int IsMovingParam = Animator.StringToHash("IsMoving");

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Rigidbody playerBody;

    [Header("Sprites")]
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite moveSprite;
    [SerializeField] private Sprite shootSprite;

    [Header("Facing")]
    [SerializeField] private bool flipSpriteWithAim = true;
    [SerializeField] private bool useFacingPositions = false;
    [SerializeField] private Vector3 faceRightLocalPosition;
    [SerializeField] private Vector3 faceLeftLocalPosition;

    [Header("Tuning")]
    [SerializeField] private float moveThreshold = 0.1f;
    [SerializeField] private float shootFlashDuration = 0.06f;

    private Sprite initialSprite;
    private float shootFlashTimer;
    private bool hasShootTrigger;
    private bool hasIsMovingParameter;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (playerController == null)
        {
            playerController = GetComponentInParent<PlayerController>();
        }

        if (playerBody == null)
        {
            playerBody = GetComponentInParent<Rigidbody>();
        }

        if (spriteRenderer != null)
        {
            initialSprite = spriteRenderer.sprite;
        }

        InitializeFacingPositions();
        CacheAnimatorParameters();
    }

    private void LateUpdate()
    {
        if (spriteRenderer == null)
        {
            return;
        }

        bool isMoving = IsMoving();
        UpdateFacing();

        if (animator != null && hasIsMovingParameter)
        {
            animator.SetBool(IsMovingParam, isMoving);
        }

        if (animator != null)
        {
            return;
        }

        if (shootFlashTimer > 0f)
        {
            shootFlashTimer -= Time.deltaTime;
            SetSprite(shootSprite != null ? shootSprite : GetBaseSprite(isMoving));
            return;
        }

        SetSprite(GetBaseSprite(isMoving));
    }

    public void PlayShootFlash()
    {
        if (animator != null && hasShootTrigger)
        {
            animator.ResetTrigger(ShootParam);
            animator.SetTrigger(ShootParam);
            return;
        }

        if (shootSprite == null)
        {
            return;
        }

        shootFlashTimer = shootFlashDuration;
        SetSprite(shootSprite);
    }

    private Sprite GetBaseSprite(bool isMoving)
    {
        if (isMoving && moveSprite != null)
        {
            return moveSprite;
        }

        if (idleSprite != null)
        {
            return idleSprite;
        }

        return initialSprite;
    }

    private void UpdateFacing()
    {
        if (playerController == null)
        {
            return;
        }

        Vector3 aimDirection = playerController.GetAimDirection();

        if (aimDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        bool facingRight = aimDirection.x >= 0f;

        if (flipSpriteWithAim)
        {
            spriteRenderer.flipX = !facingRight;
        }

        if (useFacingPositions)
        {
            transform.localPosition = facingRight ? faceRightLocalPosition : faceLeftLocalPosition;
        }
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

    private void CacheAnimatorParameters()
    {
        hasShootTrigger = HasParameter("Shoot", AnimatorControllerParameterType.Trigger);
        hasIsMovingParameter = HasParameter("IsMoving", AnimatorControllerParameterType.Bool);
    }

    private bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
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
