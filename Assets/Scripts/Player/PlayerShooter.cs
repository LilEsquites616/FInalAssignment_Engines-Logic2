using UnityEngine;
using System.Collections;
using TMPro;

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

    [Header("Ammo")]
    [SerializeField] private int maxAmmo = 30;
    [SerializeField] private int currentAmmo;
    [SerializeField] private float reloadTime = 1.5f;
    [SerializeField] private TMP_Text ammoText;

    private float lastShotTime = -999f;
    private Vector2 lookInput;
    private bool isReloading = false;

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

    
    if (ModsManager.Instance != null && ModsManager.Instance.ammoActive)
    {
        maxAmmo += 20;
        Debug.Log("Ammo Power-Up Active: +20 max ammo");
    }

    currentAmmo = maxAmmo;
    UpdateAmmoUI();
}

    private void Update()
    {
        UpdateAim();
        AutoFireWhileAiming();
    }

    private void AutoFireWhileAiming()
    {
        if (lookInput.sqrMagnitude > 0.01f)
        {
            TryShoot();
            return;
        }
    }

    private void UpdateAim()
    {
        if (lookInput.sqrMagnitude > 0.5f)
        {
            Vector3 aimDirection = new Vector3(lookInput.x, 0f, lookInput.y);
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

        Plane groundPlane = new Plane(
            Vector3.up,
            new Vector3(0f, firePoint != null ? firePoint.position.y : transform.position.y, 0f)
        );

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

#if ENABLE_INPUT_SYSTEM
    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();

        if (lookInput.magnitude < 0.2f)
        {
            lookInput = Vector2.zero;
        }
    }

    public void OnReload(InputValue value)
    {
        if (value.isPressed)
        {
            StartCoroutine(Reload());
        }
    }
#endif

    private void TryShoot()
    {
        if (isReloading) return;

        if (Time.time - lastShotTime < fireCooldown)
        {
            return;
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        Vector3 shootDirection = transform.forward;

        if (lookInput.sqrMagnitude > 0.01f)
        {
            shootDirection = new Vector3(lookInput.x, 0f, lookInput.y).normalized;
        }

        else if (TryGetMouseAimPoint(out Vector3 aimPoint))
        {
            shootDirection = (aimPoint - firePoint.position).normalized;
        }
        
        else if (playerController != null)
        {
            shootDirection = playerController.GetAimDirection();
        }

        if (shootDirection.sqrMagnitude < 0.01f)
        {
            return;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(shootDirection, Vector3.up)
        );

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
            foreach (var col in playerColliders)
            {
                Physics.IgnoreCollision(bulletCollider, col, true);
            }
        }

        currentAmmo--;
        UpdateAmmoUI();

        lastShotTime = Time.time;
    }

    private IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + maxAmmo;
        }
    }
}