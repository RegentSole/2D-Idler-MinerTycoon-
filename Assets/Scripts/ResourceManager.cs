using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int resource1 = 0;           // Текущие единицы ресурса 1
    public int resource2 = 500;         // Текущие единицы ресурса 2
    public bool autoCollectorPurchased = false; // Флаг, показывающий, куплена ли система авто-сбора
    public float autoCollectionInterval = 5f;    // Интервал авто-сбора в секундах
    public int autoCollectionAmount = 1;        // Количество ресурса 1, добавляемое при авто-сборе
    public int purchaseCost = 100;              // Стоимость покупки авто-системы сбора

    private float nextAutoCollectionTime;       // Время следующего авто-сбора

    void Start()
    {
        nextAutoCollectionTime = Time.time + autoCollectionInterval;
    }

    void Update()
    {
        if (autoCollectorPurchased && Time.time >= nextAutoCollectionTime)
        {
            CollectResource1(autoCollectionAmount); // Собираем ресурс 1
            nextAutoCollectionTime = Time.time + autoCollectionInterval; // Обновляем время следующего сбора
        }
    }

    // Метод для покупки авто-системы сбора
    public void PurchaseAutoCollector()
    {
        if (resource2 >= purchaseCost)
        {
            resource2 -= purchaseCost;          // Снимаем стоимость покупки
            autoCollectorPurchased = true;      // Включаем авто-сбор
            Debug.Log("Auto collector purchased!");
        }
        else
        {
            Debug.Log("Not enough resource 2 to purchase the auto collector.");
        }
    }

    // Метод для ручного сбора ресурса 1
    public void CollectResource1(int amount)
    {
        resource1 += amount;
        Debug.Log($"Collected {amount} units of resource 1. Total: {resource1}");
    }

    // Метод для проверки наличия достаточного количества ресурса 2 для покупки авто-системы
    public bool CanPurchaseAutoCollector()
    {
        return resource2 >= purchaseCost;
    }

    // Методы для получения текущих значений ресурсов
    public int GetResource1()
    {
        return resource1;
    }

    public int GetResource2()
    {
        return resource2;
    }
}

