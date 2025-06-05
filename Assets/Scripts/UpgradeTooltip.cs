using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UpgradeTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")] [SerializeField] private GameObject tooltipPrefab;[SerializeField] private UpgradeData upgradeData;[SerializeField] private UpgradeButton linkedUpgradeButton;


    [Header("Settings")]
    [SerializeField] private Vector2 positionOffset = new Vector2(20, -20);
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private bool debugMode = false;

    private GameObject activeTooltip;
    private Coroutine fadeRoutine;
    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (!parentCanvas) Debug.LogError("Parent Canvas not found!", this);
    }

    void OnDisable() => DestroyTooltip();

    public void OnPointerEnter(PointerEventData eventData) => StartCoroutine(ShowTooltip());
    public void OnPointerExit(PointerEventData eventData) => StartCoroutine(HideTooltip());

    private IEnumerator ShowTooltip()
    {
        if (!ValidateReferences() || activeTooltip != null) yield break;

        // Создаем tooltip
        activeTooltip = Instantiate(tooltipPrefab, parentCanvas.transform);
        activeTooltip.transform.SetAsLastSibling();

        // Инициализация компонентов
        RectTransform tooltipRect = activeTooltip.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = activeTooltip.GetComponent<CanvasGroup>();

        if (!InitializeComponents(tooltipRect, canvasGroup)) yield break;

        // Позиционирование
        PositionTooltip(tooltipRect);

        // Настройка контента
        InitializeContent();

        // Анимация появления
        yield return StartCoroutine(FadeAnimation(canvasGroup, 0, 1));
    }

    private IEnumerator HideTooltip()
    {
        if (activeTooltip == null) yield break;

        CanvasGroup canvasGroup = activeTooltip.GetComponent<CanvasGroup>();
        yield return StartCoroutine(FadeAnimation(canvasGroup, 1, 0));

        DestroyTooltip();
    }

    #region Helper Methods
    private bool ValidateReferences()
    {
        if (!tooltipPrefab) LogError("Tooltip prefab is not assigned!");
        if (!upgradeData) LogError("UpgradeData is not assigned!");
        if (!linkedUpgradeButton) LogError("Linked UpgradeButton is not assigned!");
        return tooltipPrefab && upgradeData && linkedUpgradeButton;
    }

    private bool InitializeComponents(RectTransform tooltipRect, CanvasGroup canvasGroup)
    {
        if (!tooltipRect)
        {
            LogError("Tooltip is missing RectTransform component!");
            return false;
        }

        if (!canvasGroup)
        {
            LogError("Tooltip is missing CanvasGroup component!");
            return false;
        }

        canvasGroup.alpha = 0;
        return true;
    }

    private void PositionTooltip(RectTransform tooltipRect)
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPoint;

        // Преобразование координат мыши в локальные координаты Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.GetComponent<RectTransform>(),
            mousePosition,
            parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : parentCanvas.worldCamera,
            out localPoint
        );

        // Рассчитываем позицию с ограничениями
        Vector2 clampedPosition = CalculateClampedPosition(localPoint, tooltipRect);
        tooltipRect.localPosition = clampedPosition;
    }

    private Vector2 CalculateClampedPosition(Vector2 targetPosition, RectTransform tooltipRect)
    {
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        Vector2 canvasSize = canvasRect.rect.size;
        Vector2 tooltipSize = tooltipRect.rect.size;

        float maxX = (canvasSize.x / 2) - tooltipSize.x / 2;
        float minX = -maxX;
        float maxY = (canvasSize.y / 2) - tooltipSize.y / 2;
        float minY = -maxY;

        return new Vector2(
            Mathf.Clamp(targetPosition.x + positionOffset.x, minX, maxX),
            Mathf.Clamp(targetPosition.y + positionOffset.y, minY, maxY)
        );
    }

    private void InitializeContent()
    {
        TMP_Text titleText = activeTooltip.transform.Find("TitleText")?.GetComponent<TMP_Text>();
        TMP_Text priceText = activeTooltip.transform.Find("PriceText")?.GetComponent<TMP_Text>();
        TMP_Text descText = activeTooltip.transform.Find("DescriptionText")?.GetComponent<TMP_Text>();

        if (titleText == null || priceText == null || descText == null)
        {
            LogError("Tooltip is missing required text components!");
            DestroyTooltip();
            return;
        }

        titleText.text = upgradeData.upgradeName;
        descText.text = upgradeData.description;

        // Проверка наличия linkedUpgradeButton
        if (linkedUpgradeButton != null)
        {
            priceText.text = $"{upgradeData.GetCurrentCost(linkedUpgradeButton.CurrentLevel)} {upgradeData.currencyResource}";
        }
        else
        {
            priceText.text = "N/A";
            LogError("Linked UpgradeButton is not assigned!");
        }
    }

    private IEnumerator FadeAnimation(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    private void DestroyTooltip()
    {
        if (activeTooltip)
        {
            Destroy(activeTooltip);
            activeTooltip = null;
        }
    }

    private void LogError(string message)
    {
        if (debugMode) Debug.LogError(message, this);
    }
    #endregion
}