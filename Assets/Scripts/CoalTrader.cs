using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoalTrader : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text coalAmountText;
    [SerializeField] private TMP_Text coinAmountText;
    [SerializeField] private Button sellButton;
    [SerializeField] private Slider amountSlider;

    [Header("Trade Settings")]
    [SerializeField] private int coalPerCoin = 10;
    [SerializeField] private bool alwaysShowSlider = true;

    private int maxCoalToSell = 0;
    private int selectedCoalAmount = 0;

    void Start()
    {
        amountSlider.onValueChanged.AddListener(UpdateTradeUI);
        sellButton.onClick.AddListener(SellCoal);
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
        sellButton.interactable = false;

        // Начальная настройка слайдера
        amountSlider.minValue = 0;
        amountSlider.wholeNumbers = true;

        if (alwaysShowSlider) amountSlider.gameObject.SetActive(true);
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    private void OnResourceChanged(ResourceType resourceType)
    {
        if (resourceType == ResourceType.Coal || resourceType == ResourceType.Coin)
        {
            UpdateTradeUIState();
        }
    }

    private void UpdateTradeUIState()
    {
        maxCoalToSell = ResourceManager.Instance.GetResourceCount(ResourceType.Coal);
        amountSlider.maxValue = maxCoalToSell;

        // Рассчитываем максимальное количество для целых монет
        int maxTradeableCoal = maxCoalToSell - (maxCoalToSell % coalPerCoin);

        // Если угля меньше, чем нужно для одной монеты
        if (maxTradeableCoal < coalPerCoin)
        {
            // Сбрасываем выбор
            amountSlider.value = 0;
            sellButton.interactable = false;
        }
        else
        {
            // Ограничиваем выбор до количества, дающего целые монеты
            if (selectedCoalAmount > maxTradeableCoal)
            {
                amountSlider.value = maxTradeableCoal;
            }
            sellButton.interactable = (selectedCoalAmount >= coalPerCoin);
        }

        // Обновляем текстовые поля
        UpdateTradeUI(amountSlider.value);
    }

    private void UpdateTradeUI(float value)
    {
        selectedCoalAmount = Mathf.RoundToInt(value);

        // Рассчитываем сколько целых монет можно получить
        int possibleCoins = selectedCoalAmount / coalPerCoin;
        int coalUsed = possibleCoins * coalPerCoin;
        int coalLeft = selectedCoalAmount - coalUsed;

        coalAmountText.text = $"{selectedCoalAmount}";
        coinAmountText.text = $"{possibleCoins}";

        // Подсветка, если угля недостаточно для целой монеты
        coalAmountText.color = coalLeft > 0 ? Color.yellow : Color.white;

        // Кнопка активна только если можно получить хотя бы 1 монету
        sellButton.interactable = (possibleCoins >= 1);
    }

    private void SellCoal()
    {
        if (selectedCoalAmount < coalPerCoin) return;

        // Рассчитываем сколько монет можно получить (только целые)
        int coalToSell = (selectedCoalAmount / coalPerCoin) * coalPerCoin;
        int coinsToReceive = coalToSell / coalPerCoin;

        if (ResourceManager.Instance.TradeResources(
            ResourceType.Coal,
            coalToSell,
            ResourceType.Coin,
            coinsToReceive))
        {
            // Сброс слайдера после успешной продажи
            amountSlider.value = 0;
            selectedCoalAmount = 0;
            UpdateTradeUI(0);
        }
    }

    // Для отладки в инспекторе
    [ContextMenu("Test Trade 9 Coal")]
    void TestTrade9Coal()
    {
        ResourceManager.Instance.AddResource(ResourceType.Coal, 9);
        UpdateTradeUIState();
    }

    [ContextMenu("Test Trade 10 Coal")]
    void TestTrade10Coal()
    {
        ResourceManager.Instance.AddResource(ResourceType.Coal, 10);
        UpdateTradeUIState();
    }

    [ContextMenu("Test Trade 19 Coal")]
    void TestTrade19Coal()
    {
        ResourceManager.Instance.AddResource(ResourceType.Coal, 19);
        UpdateTradeUIState();
    }
}