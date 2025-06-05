using UnityEngine;
using UnityEngine.UI;

public class PurchaseEffect : MonoBehaviour
{
    [Header("Particle Settings")]
    public ParticleSystem banknoteParticlePrefab;
    public int minParticles = 8;
    public int maxParticles = 15;
    public float particleSize = 0.5f;
    public float spreadAngle = 60f;

    private Button purchaseButton;

    void Start()
    {
        purchaseButton = GetComponent<Button>();
        purchaseButton.onClick.AddListener(OnPurchaseClicked);

        // Инициализация пула при старте
        if (ParticlePool2D.Instance != null && banknoteParticlePrefab != null)
        {
            ParticlePool2D.Instance.particlePrefab = banknoteParticlePrefab;
            ParticlePool2D.Instance.InitializePool();
        }
    }

    public void OnPurchaseClicked()
    {
        // Ваша логика покупки
        Debug.Log("Улучшение куплено!");

        // Запуск эффекта банкнот
        PlayBanknoteEffect();
    }

    private void PlayBanknoteEffect()
    {
        if (ParticlePool2D.Instance == null || banknoteParticlePrefab == null) return;

        // Получаем позицию кнопки
        Vector3 spawnPosition = GetButtonPosition();

        // Получаем систему частиц
        ParticleSystem ps = ParticlePool2D.Instance.GetParticleSystem();
        if (ps == null) return;

        // Настраиваем
        ConfigureParticles(ps);
        ps.transform.position = spawnPosition;
        ps.Play();

        // Возвращаем в пул после завершения
        ParticlePool2D.Instance.ReturnAfterDelay(ps, ps.main.duration);
    }

    private void ConfigureParticles(ParticleSystem ps)
    {
        var emission = ps.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, Random.Range(minParticles, maxParticles)));

        var main = ps.main;
        main.startSize = particleSize;

        var shape = ps.shape;
        shape.angle = spreadAngle;
    }

    private Vector3 GetButtonPosition()
    {
        // Для UI кнопок
        if (TryGetComponent<RectTransform>(out RectTransform rect))
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                return rect.position;
            }
            return rect.TransformPoint(rect.rect.center);
        }

        // Для 2D объектов
        return transform.position;
    }
}