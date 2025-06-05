using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] private UpgradeData upgradeData;
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
        int cost = CalculateCurrentCost();

        if (ResourceManager.Instance.CanAfford(upgradeData.currencyResource, cost))
        {
            ResourceManager.Instance.SpendResources(upgradeData.currencyResource, cost);
            ResourceManager.Instance.AddProductionMultiplier(
                upgradeData.targetResource,
                upgradeData.productionBonusPerLevel
            );
            currentLevel++;
            UpdateUI();
        }
    }

    private int CalculateCurrentCost()
    {
        return Mathf.RoundToInt(upgradeData.baseCost *
            Mathf.Pow(upgradeData.costMultiplier, currentLevel));
    }

    private void UpdateUI()
    {
        titleText.text = $"{upgradeData.upgradeName} (Lv.{currentLevel + 1})";
        descriptionText.text = $"+{upgradeData.productionBonusPerLevel * 1} к добыче";
        costText.text = $"{CalculateCurrentCost()} {upgradeData.currencyResource}";

        buyButton.interactable = ResourceManager.Instance.CanAfford(
            upgradeData.currencyResource,
            CalculateCurrentCost()
        );
    }
}