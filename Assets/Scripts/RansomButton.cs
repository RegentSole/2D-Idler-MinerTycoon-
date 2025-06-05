using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RansomButton : MonoBehaviour
{
    [Header("Settings")]
    public int ransomCost = 1000;
    public Button ransomButton;
    public TextMeshProUGUI costText;

    [Header("Win Screen")]
    public GameObject winScreen;

    void Start()
    {
        // Назначаем обработчик клика
        ransomButton.onClick.AddListener(OnRansomClick);

        // Обновляем текст стоимости
        UpdateCostText();
    }

    void Update()
    {
        // Проверяем состояние кнопки каждый кадр
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        // Получаем количество монет - ИСПРАВЛЕНО: используем GetResourceCount
        int coins = ResourceManager.Instance.GetResourceCount(ResourceType.Coin);

        // Активируем кнопку только если достаточно монет
        ransomButton.interactable = coins >= ransomCost;

        // Обновляем текст кнопки
        UpdateCostText();
    }

    private void UpdateCostText()
    {
        if (costText != null)
        {
            // Показываем сколько монет нужно, если недостаточно
            int coins = ResourceManager.Instance.GetResourceCount(ResourceType.Coin);
            if (coins < ransomCost)
            {
                costText.text = $"Нужно на {ransomCost - coins} монет больше";
            }
            else
            {
                costText.text = $"Заплатить {ransomCost} за Выкуп";
            }
        }
    }

    private void OnRansomClick()
    {
        // Проверяем еще раз на случай если состояние изменилось
        int coins = ResourceManager.Instance.GetResourceCount(ResourceType.Coin);
        if (coins < ransomCost) return;

        // Списание монет - ИСПРАВЛЕНО: используем SpendResources
        ResourceManager.Instance.SpendResources(ResourceType.Coin, ransomCost);

        // Показать экран победы
        ShowWinScreen();
    }

    private void ShowWinScreen()
    {
        if (winScreen != null)
        {
            winScreen.SetActive(true);

            // Останавливаем игру
            Time.timeScale = 0f;
        }
    }
}