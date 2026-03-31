using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody body;
    [SerializeField] private Collider bodyCollider;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform rotationRoot;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float rotationSpeed = 1080f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5.5f;
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private float riseGravityMultiplier = 2f;
    [SerializeField] private float fallGravityMultiplier = 4f;

    private Vector2 moveInput;
    private Vector3 aimDirection = Vector3.forward;
    private bool jumpRequested;
    private bool hasPlayerInput;

    private void Awake()
    {
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider>();
        }

        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        if (rotationRoot == null)
        {
            rotationRoot = transform;
        }

#if ENABLE_INPUT_SYSTEM
        hasPlayerInput = GetComponent<PlayerInput>() != null;
#endif

        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        ReadFallbackInput();
        UpdateAimFromCursor();
        RotateTowardsAim();
    }

    private void FixedUpdate()
    {
        Move();
        TryJump();
        ApplyExtraGravity();
        body.angularVelocity = Vector3.zero;
    }

    private void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 currentHorizontalVelocity = new Vector3(body.linearVelocity.x, 0f, body.linearVelocity.z);
        Vector3 targetHorizontalVelocity = moveDirection * moveSpeed;
        float maxSpeedChange = (moveDirection.sqrMagnitude > 0.001f ? acceleration : deceleration) * Time.fixedDeltaTime;
        Vector3 nextHorizontalVelocity = Vector3.MoveTowards(currentHorizontalVelocity, targetHorizontalVelocity, maxSpeedChange);

        body.linearVelocity = new Vector3(
            nextHorizontalVelocity.x,
            body.linearVelocity.y,
            nextHorizontalVelocity.z);
    }

    private void RotateTowardsAim()
    {
        Vector3 flattenedAimDirection = new Vector3(aimDirection.x, 0f, aimDirection.z);

        if (flattenedAimDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(flattenedAimDirection.normalized, Vector3.up);
        rotationRoot.rotation = Quaternion.RotateTowards(
            rotationRoot.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    private void TryJump()
    {
        if (!jumpRequested)
        {
            return;
        }

        jumpRequested = false;

        if (!IsGrounded())
        {
            return;
        }

        Vector3 velocity = body.linearVelocity;
        velocity.y = 0f;
        body.linearVelocity = velocity;
        body.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ApplyExtraGravity()
    {
        if (body.linearVelocity.y > 0f)
        {
            Vector3 extraRiseGravity = Physics.gravity * (riseGravityMultiplier - 1f);
            body.AddForce(extraRiseGravity, ForceMode.Acceleration);
            return;
        }

        if (body.linearVelocity.y < 0f)
        {
            Vector3 extraFallGravity = Physics.gravity * (fallGravityMultiplier - 1f);
            body.AddForce(extraFallGravity, ForceMode.Acceleration);
        }
    }

    private void UpdateAimFromCursor()
    {
        if (aimCamera == null)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null)
        {
            return;
        }

        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
        Ray ray = aimCamera.ScreenPointToRay(Input.mousePosition);
#endif

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (!groundPlane.Raycast(ray, out float enter))
        {
            return;
        }

        Vector3 aimPoint = ray.GetPoint(enter);
        Vector3 direction = aimPoint - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        aimDirection = direction.normalized;
    }

    private bool IsGrounded()
    {
        if (bodyCollider == null)
        {
            return true;
        }

        Bounds bounds = bodyCollider.bounds;
        Vector3 origin = bounds.center;
        float radius = Mathf.Max(0.05f, Mathf.Min(bounds.extents.x, bounds.extents.z) * 0.8f);
        float castDistance = bounds.extents.y + groundCheckDistance;

        return Physics.SphereCast(origin, radius, Vector3.down, out _, castDistance, ~0, QueryTriggerInteraction.Ignore);
    }

    public void SetAimDirection(Vector3 worldDirection)
    {
        worldDirection.y = 0f;

        if (worldDirection.sqrMagnitude < 0.001f)
        {
            return;
        }

        aimDirection = worldDirection.normalized;
    }

    public Vector3 GetAimDirection()
    {
        return aimDirection.sqrMagnitude < 0.001f ? rotationRoot.forward : aimDirection.normalized;
    }

    public void Jump()
    {
        jumpRequested = true;
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        moveInput = Vector2.ClampMagnitude(value.Get<Vector2>(), 1f);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            Jump();
        }
    }

    private void ReadFallbackInput()
    {
        if (hasPlayerInput)
        {
            return;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) input.y -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input.x += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input.x -= 1f;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Jump();
            }
        }

        if (Gamepad.current != null && Gamepad.current.leftStick.ReadValue().sqrMagnitude > input.sqrMagnitude)
        {
            input = Gamepad.current.leftStick.ReadValue();
        }

        moveInput = Vector2.ClampMagnitude(input, 1f);
    }
#else
    private void ReadFallbackInput()
    {
        moveInput = Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }
#endif
}
