using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [System.Serializable]
    public class ResourceUI
    {
        public ResourceType resourceType;
        public TMP_Text resourceText;
        public string displayFormat = "{0}";
    }

    public List<ResourceUI> resourceUIs = new List<ResourceUI>();

    [Header("Ransom Settings")]
    public int ransomAmount = 1000;
    public ResourceType ransomCurrency = ResourceType.Coin;
    public event Action<bool> OnRansomAvailabilityChanged;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private Dictionary<ResourceType, float> productionMultipliers = new Dictionary<ResourceType, float>();
    private Dictionary<ResourceType, float> autoProductionRates = new Dictionary<ResourceType, float>();
    private Dictionary<ResourceType, float> productionAccumulators = new Dictionary<ResourceType, float>();

    public event Action<ResourceType> OnResourceChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeResources();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeResources()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
            productionMultipliers[type] = 1f;
            autoProductionRates[type] = 0f;
            productionAccumulators[type] = 0f;
            UpdateResourceText(type);
        }

        // Проверяем доступность выкупа при инициализации
        CheckRansomAvailability();
    }

    void Update()
    {
        // Обработка автодобычи
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            if (autoProductionRates[type] > 0)
            {
                productionAccumulators[type] += autoProductionRates[type] * Time.deltaTime;

                if (productionAccumulators[type] >= 1)
                {
                    int amountToAdd = Mathf.FloorToInt(productionAccumulators[type]);
                    AddResource(type, amountToAdd);
                    productionAccumulators[type] -= amountToAdd;
                }
            }
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        resources[type] += amount;
        UpdateResourceText(type);
        OnResourceChanged?.Invoke(type);

        // Проверяем доступность выкупа при изменении ресурсов
        if (type == ransomCurrency)
        {
            CheckRansomAvailability();
        }
    }

    private void CheckRansomAvailability()
    {
        bool isAvailable = CanAffordRansom();
        OnRansomAvailabilityChanged?.Invoke(isAvailable);
    }

    public bool CanAffordRansom()
    {
        return GetResourceCount(ransomCurrency) >= ransomAmount;
    }

    public bool PayRansom()
    {
        if (!CanAffordRansom()) return false;

        SpendResources(ransomCurrency, ransomAmount);
        return true;
    }

    public float GetProductionMultiplier(ResourceType type)
    {
        return productionMultipliers.ContainsKey(type) ? productionMultipliers[type] : 1f;
    }

    public void AddProductionMultiplier(ResourceType type, float multiplier)
    {
        if (productionMultipliers.ContainsKey(type))
        {
            productionMultipliers[type] += multiplier;
        }
        else
        {
            productionMultipliers[type] = 1f + multiplier;
        }
    }

    public void SetAutoProductionRate(ResourceType type, float ratePerSecond)
    {
        if (autoProductionRates.ContainsKey(type))
        {
            autoProductionRates[type] = ratePerSecond;
        }
        else
        {
            autoProductionRates[type] = ratePerSecond;
        }
    }

    public float GetAutoProductionRate(ResourceType type)
    {
        return autoProductionRates.ContainsKey(type) ? autoProductionRates[type] : 0f;
    }

    public int GetResourceCount(ResourceType type)
    {
        return resources.ContainsKey(type) ? resources[type] : 0;
    }

    public bool CanAfford(ResourceType type, int amount)
    {
        return GetResourceCount(type) >= amount;
    }

    public void SpendResources(ResourceType type, int amount)
    {
        if (CanAfford(type, amount))
        {
            resources[type] -= amount;
            UpdateResourceText(type);
            OnResourceChanged?.Invoke(type);

            if (type == ransomCurrency)
            {
                CheckRansomAvailability();
            }
        }
    }

    public bool TradeResources(ResourceType sellResource, int sellAmount, ResourceType buyResource, int buyAmount)
    {
        if (!CanAfford(sellResource, sellAmount)) return false;

        SpendResources(sellResource, sellAmount);
        AddResource(buyResource, buyAmount);
        return true;
    }

    private void UpdateResourceText(ResourceType type)
    {
        foreach (var resourceUI in resourceUIs)
        {
            if (resourceUI.resourceType == type)
            {
                resourceUI.resourceText.text = string.Format(
                    resourceUI.displayFormat,
                    resources[type]
                );
                return;
            }
        }
    }

    public void ResetAll()
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
            productionMultipliers[type] = 1f;
            autoProductionRates[type] = 0f;
            productionAccumulators[type] = 0f;
            UpdateResourceText(type);
        }

        // Проверяем доступность выкупа после сброса
        CheckRansomAvailability();
    }

    // Для дебага
    public void DebugAddResources(int amount)
    {
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            AddResource(type, amount);
        }
    }
}