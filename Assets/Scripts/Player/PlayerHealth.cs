using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class PlayerHealth : MonoBehaviour
{
    public float baseMaxHealth = 100f;
    public float maxHealth;
    public float currentHealth;
    public AdManager adManager;
    public TextMeshProUGUI healthText;

    [Header("Shield Visual")]
    [SerializeField] private Color shieldColor = new Color(0.2f, 0.7f, 1f, 0.2f);
    [SerializeField] private float shieldScalePadding = 0.35f;

    private float damageMultiplier = 1f;
    private Coroutine shieldRoutine;
    private GameObject shieldVisual;
    private Material shieldMaterial;

    private void Awake()
    {
        maxHealth = baseMaxHealth;
        if (ModsManager.Instance != null && ModsManager.Instance.hpActive)
        {
            maxHealth += 50f;

        }

        currentHealth = maxHealth;
        CreateShieldVisual();
        SetShieldVisualActive(false);
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount * damageMultiplier;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");
        if (GameOverHandler.Instance != null)
        {
            GameOverHandler.Instance.TriggerGameOver(false);
        }

        if (adManager != null)
        {
            adManager.LoadAd("Interstitial");
            adManager.ShowAd("Interstitial");
        }
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}";
    }

    public void ApplyTemporaryShield(float incomingDamageMultiplier, float duration)
    {
        if (shieldRoutine != null)
        {
            StopCoroutine(shieldRoutine);
        }

        shieldRoutine = StartCoroutine(TemporaryShieldRoutine(incomingDamageMultiplier, duration));
    }

    private IEnumerator TemporaryShieldRoutine(float incomingDamageMultiplier, float duration)
    {
        damageMultiplier = Mathf.Clamp(incomingDamageMultiplier, 0f, 1f);
        SetShieldVisualActive(true);

        yield return new WaitForSeconds(duration);

        damageMultiplier = 1f;
        SetShieldVisualActive(false);
        shieldRoutine = null;
    }

    private void CreateShieldVisual()
    {
        if (shieldVisual != null)
        {
            return;
        }

        GameObject visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visualObject.name = "ShieldVisual";
        visualObject.transform.SetParent(transform, false);
        visualObject.transform.localPosition = GetVisualCenterOffset();
        visualObject.transform.localRotation = Quaternion.identity;
        visualObject.transform.localScale = Vector3.one * GetVisualScale();

        Collider shieldCollider = visualObject.GetComponent<Collider>();
        if (shieldCollider != null)
        {
            Destroy(shieldCollider);
        }

        Renderer renderer = visualObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            shieldMaterial = CreateShieldMaterial();
            if (shieldMaterial != null)
            {
                renderer.material = shieldMaterial;
            }
        }

        shieldVisual = visualObject;
    }

    private Material CreateShieldMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Standard");
        }
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Color");
        }
        if (shader == null)
        {
            return null;
        }

        Material material = new Material(shader);

        if (material.HasProperty("_BaseColor"))
        {
            material.SetColor("_BaseColor", shieldColor);
        }

        if (material.HasProperty("_Color"))
        {
            material.SetColor("_Color", shieldColor);
        }

        if (shader.name == "Universal Render Pipeline/Unlit")
        {
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 0f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
        }
        else if (shader.name == "Standard")
        {
            material.SetFloat("_Mode", 3f);
            material.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
            material.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
            material.SetInt("_ZWrite", 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int)RenderQueue.Transparent;
        }

        return material;
    }

    private Vector3 GetVisualCenterOffset()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return Vector3.up;
        }

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        return transform.InverseTransformPoint(combinedBounds.center);
    }

    private float GetVisualScale()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return 2f;
        }

        Bounds combinedBounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            combinedBounds.Encapsulate(renderers[i].bounds);
        }

        float maxExtent = Mathf.Max(combinedBounds.extents.x, combinedBounds.extents.y, combinedBounds.extents.z);
        return (maxExtent * 2f) + shieldScalePadding;
    }

    private void SetShieldVisualActive(bool isActive)
    {
        if (shieldVisual == null)
        {
            return;
        }

        shieldVisual.SetActive(isActive);
    }
}
