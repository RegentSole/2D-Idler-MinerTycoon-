using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AutoProductionUpgradeButton : MonoBehaviour
{
    [SerializeField] private AutoProductionUpgradeData upgradeData;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Button buyButton;

    private int currentLevel = 0;
    public int CurrentLevel => currentLevel; // Публичное свойство

    void Start()
    {
        UpdateUI();
        ResourceManager.Instance.OnResourceChanged += OnResourceChanged;
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
        if (resourceType == upgradeData.currencyResource)
        {
            UpdateUI();
        }
    }

    public void PurchaseUpgrade()
    {
        int cost = upgradeData.GetCurrentCost(currentLevel);

        if (ResourceManager.Instance.CanAfford(upgradeData.currencyResource, cost))
        {
            ResourceManager.Instance.SpendResources(upgradeData.currencyResource, cost);

            // Рассчитываем новую скорость добычи
            float newRate = upgradeData.productionPerSecond * (currentLevel + 1);

            // Устанавливаем новую скорость
            ResourceManager.Instance.SetAutoProductionRate(
                upgradeData.targetResource,
                newRate
            );

            currentLevel++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        titleText.text = $"{upgradeData.upgradeName} (Lv.{currentLevel})";
        descriptionText.text = $"Добыча: {upgradeData.productionPerSecond * (currentLevel)}/сек";
        costText.text = $"{upgradeData.GetCurrentCost(currentLevel)} {upgradeData.currencyResource}";

        buyButton.interactable = ResourceManager.Instance.CanAfford(
            upgradeData.currencyResource,
            upgradeData.GetCurrentCost(currentLevel)
        );
    }
}