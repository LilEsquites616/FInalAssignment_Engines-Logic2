using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerShooter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private float bulletDamage = 20f;
    [SerializeField] private LayerMask aimLayers = ~0;

    private float lastShotTime = -999f;
    private Vector2 lookInput;
    private bool hasPlayerInput;

    private void Awake()
    {
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

#if ENABLE_INPUT_SYSTEM
        hasPlayerInput = GetComponent<PlayerInput>() != null;
#endif
    }

    private void Update()
    {
        UpdateAim();
        ReadFireFallbackInput();
    }

    private void UpdateAim()
    {
        Vector3 aimDirection = Vector3.zero;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null && lookInput.sqrMagnitude > 0.01f)
        {
            aimDirection = new Vector3(lookInput.x, 0f, lookInput.y);
        }
#endif

        if (aimDirection.sqrMagnitude >= 0.01f)
        {
            playerController?.SetAimDirection(aimDirection.normalized);
        }
    }

    private bool TryGetMouseAimPoint(out Vector3 aimPoint)
    {
        aimPoint = Vector3.zero;

        if (aimCamera == null)
        {
            return false;
        }

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current == null)
        {
            return false;
        }

        Ray ray = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
#else
        Ray ray = aimCamera.ScreenPointToRay(Input.mousePosition);
#endif

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, firePoint != null ? firePoint.position.y : transform.position.y, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, 500f, aimLayers, QueryTriggerInteraction.Ignore))
        {
            aimPoint = hit.point;
            return true;
        }

        if (groundPlane.Raycast(ray, out float enter))
        {
            aimPoint = ray.GetPoint(enter);
            return true;
        }

        return false;
    }

    public void Fire()
    {
        TryShoot();
    }

#if ENABLE_INPUT_SYSTEM
    public void OnLook(InputValue value)
    {
#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            lookInput = Vector2.zero;
            return;
        }
#endif

        lookInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            TryShoot();
        }
    }

    private void ReadFireFallbackInput()
    {
        if (hasPlayerInput)
        {
            return;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryShoot();
        }

        if (Gamepad.current != null)
        {
            lookInput = Gamepad.current.rightStick.ReadValue();

            if (Gamepad.current.rightTrigger.wasPressedThisFrame || Gamepad.current.buttonWest.wasPressedThisFrame)
            {
                TryShoot();
            }
        }
        else if (lookInput.sqrMagnitude < 0.01f)
        {
            lookInput = Vector2.zero;
        }
    }
#else
    private void ReadFireFallbackInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryShoot();
        }
    }
#endif

    private void TryShoot()
    {
        if (Time.time - lastShotTime < fireCooldown)
        {
            return;
        }

        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        Vector3 shootDirection = playerController != null ? playerController.GetAimDirection() : transform.forward;

        if (TryGetMouseAimPoint(out Vector3 aimPoint))
        {
            shootDirection = (aimPoint - firePoint.position).normalized;
        }
#if ENABLE_INPUT_SYSTEM
        else if (lookInput.sqrMagnitude > 0.01f)
        {
            shootDirection = new Vector3(lookInput.x, 0f, lookInput.y).normalized;
        }
#endif

        if (shootDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection, Vector3.up));

        Bullet bulletComponent = bullet.GetComponent<Bullet>();
        if (bulletComponent != null)
        {
            bulletComponent.damage = bulletDamage;
            bulletComponent.damageEnemies = true;
            bulletComponent.damagePlayer = false;
        }

        Rigidbody bulletBody = bullet.GetComponent<Rigidbody>();
        if (bulletBody != null)
        {
            bulletBody.linearVelocity = shootDirection * bulletSpeed;
        }

        Collider bulletCollider = bullet.GetComponent<Collider>();
        Collider[] playerColliders = GetComponentsInChildren<Collider>();

        if (bulletCollider != null)
        {
            for (int i = 0; i < playerColliders.Length; i++)
            {
                Physics.IgnoreCollision(bulletCollider, playerColliders[i], true);
            }
        }

        lastShotTime = Time.time;
    }
}
