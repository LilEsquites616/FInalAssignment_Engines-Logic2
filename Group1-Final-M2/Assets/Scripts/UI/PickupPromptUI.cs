using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupPromptUI : MonoBehaviour
{
    private static PickupPromptUI instance;

    [Header("UI References")]
    [SerializeField] private TMP_Text promptText;

    [Header("Style")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = Color.red;
    [SerializeField] private float fontSize = 36f;
    [SerializeField] private Vector2 anchoredPosition = new Vector2(-32f, -32f);
    [SerializeField] private Vector2 size = new Vector2(260f, 80f);
    [SerializeField] private float blinkSpeed = 8f;
    [SerializeField] private float minBlinkAlpha = 0.2f;

    private Coroutine promptRoutine;

    public static PickupPromptUI Instance
    {
        get
        {
            if (instance == null)
            {
                CreateRuntimeInstance();
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        EnsurePromptText();
        HidePrompt();
    }

    public void ShowTimedPrompt(string label, float duration)
    {
        EnsurePromptText();

        if (promptRoutine != null)
        {
            StopCoroutine(promptRoutine);
        }

        promptRoutine = StartCoroutine(ShowTimedPromptRoutine(label, duration));
    }

    private IEnumerator ShowTimedPromptRoutine(string label, float duration)
    {
        promptText.gameObject.SetActive(true);

        float remaining = duration;

        while (remaining > 0f)
        {
            UpdatePromptVisual(label, Mathf.CeilToInt(remaining), remaining <= 3f);
            remaining -= Time.deltaTime;
            yield return null;
        }

        UpdatePromptVisual(label, 0, true);
        yield return new WaitForSeconds(0.15f);

        HidePrompt();
        promptRoutine = null;
    }

    private void UpdatePromptVisual(string label, int secondsLeft, bool isWarning)
    {
        promptText.text = $"{label} {secondsLeft}";

        Color targetColor = isWarning ? warningColor : normalColor;
        float alpha = 1f;

        if (isWarning)
        {
            alpha = Mathf.Lerp(minBlinkAlpha, 1f, Mathf.PingPong(Time.time * blinkSpeed, 1f));
        }

        targetColor.a = alpha;
        promptText.color = targetColor;
    }

    private void HidePrompt()
    {
        if (promptText == null)
        {
            return;
        }

        promptText.gameObject.SetActive(false);
        promptText.color = normalColor;
    }

    private void EnsurePromptText()
    {
        if (promptText != null)
        {
            return;
        }

        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null)
        {
            canvas = FindFirstObjectByType<Canvas>();
        }

        if (canvas == null)
        {
            GameObject canvasObject = new GameObject("PickupPromptCanvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("PickupPromptText");
        textObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = textObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = size;

        TextMeshProUGUI runtimeText = textObject.AddComponent<TextMeshProUGUI>();
        runtimeText.fontSize = fontSize;
        runtimeText.alignment = TextAlignmentOptions.TopRight;
        runtimeText.textWrappingMode = TextWrappingModes.NoWrap;

        promptText = runtimeText;
    }

    private static void CreateRuntimeInstance()
    {
        GameObject uiObject = new GameObject("PickupPromptUI");
        uiObject.AddComponent<PickupPromptUI>();
    }
}
