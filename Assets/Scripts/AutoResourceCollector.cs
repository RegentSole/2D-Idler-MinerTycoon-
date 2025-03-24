using UnityEngine;

public class AutoResourceCollector : MonoBehaviour
{
    public float collectionRate = 1f; // Скорость сбора ресурсов в секунду
    public int baseResourcesPerSecond = 1; // Базовое количество ресурсов в секунду
    private float nextCollectionTime; // Время следующего сбора ресурсов

    void Start()
    {
        nextCollectionTime = Time.time + collectionRate;
    }

    void Update()
    {
        if (Time.time >= nextCollectionTime)
        {
            AddResources(baseResourcesPerSecond);
            nextCollectionTime += collectionRate;
        }
    }

    void AddResources(int amount)
    {
        // Здесь можно добавить логику для обновления количества ресурсов
        Debug.Log("Collected " + amount + " resources");
    }
}