using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeSystem : MonoBehaviour
{
    public Button upgradeButton; // Кнопка для улучшения
    public TMP_Text upgradeCostText; // Текстовое поле для отображения стоимости улучшения
    public int currentUpgradeLevel = 1; // Текущий уровень улучшения
    public int[] upgradeCosts = { 10, 20, 30 }; // Стоимость улучшений для каждого уровня

    void Start()
    {
        upgradeButton.onClick.AddListener(Upgrade);
        UpdateUpgradeCostText();
    }

    void Upgrade()
    {
        if (currentUpgradeLevel < upgradeCosts.Length)
        {
            // Здесь можно добавить проверку наличия достаточного количества ресурсов
            currentUpgradeLevel++;
            UpdateUpgradeCostText();
            Debug.Log("Upgraded to level " + currentUpgradeLevel);
        }
    }

    void UpdateUpgradeCostText()
    {
        if (currentUpgradeLevel < upgradeCosts.Length)
        {
            upgradeCostText.text = "Upgrade Cost: " + upgradeCosts[currentUpgradeLevel];
        }
        else
        {
            upgradeCostText.text = "Max Upgrade Level Reached";
        }
    }
}