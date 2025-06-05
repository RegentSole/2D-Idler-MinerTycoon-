using UnityEngine;
using System.Collections;

public class ClickableResource : MonoBehaviour
{
    [Header("Resource Settings")]
    public ResourceType resourceType;
    public int amountPerClick = 1;

    [Header("Pulse Animation")]
    public float pulseDuration = 0.3f;
    public float pulseScale = 0.8f;
    public Color pulseColor = Color.white;
    public float colorFadeDuration = 0.1f;

    [Header("Particle Effects")]
    public ParticleSystem clickParticlesPrefab;
    public Color particleColor = Color.white;
    public float particleSizeMultiplier = 1f;
    public int minParticles = 5;
    public int maxParticles = 10;
    public float particleZOffset = -0.1f; // Смещение по Z для правильного отображения

    [Header("Sound Settings")]
    public AudioClip clickSound;
    [Range(0f, 1f)] public float soundVolume = 1f;
    [Range(0.5f, 1.5f)] public float soundPitch = 1f;
    public bool randomPitch = true;
    [Range(0f, 0.2f)] public float pitchRandomRange = 0.1f;

    private Vector3 originalScale;
    private Color originalColor;
    private SpriteRenderer spriteRenderer;
    private bool isAnimating = false;
    private ParticlePool particlePool;
    private AudioSource audioSource;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save original state
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;

        particlePool = ParticlePool.Instance;

        // Ensure particle pool exists
        if (particlePool == null && clickParticlesPrefab != null)
        {
            Debug.LogWarning("ParticlePool instance not found. Creating temporary one.");
            GameObject poolObj = new GameObject("ParticlePool");
            particlePool = poolObj.AddComponent<ParticlePool>();
            particlePool.particlePrefab = clickParticlesPrefab;
            particlePool.InitializePool();
        }

        InitializeAudio();
    }

    private void InitializeAudio()
    {
        // Try to get existing AudioSource
        audioSource = GetComponent<AudioSource>();

        // Create new if doesn't exist
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure audio source
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
        audioSource.volume = soundVolume;
    }

    void OnMouseDown()
    {
        if (isAnimating) return;

        // Play sound first
        PlayClickSound();

        // Apply production multiplier
        float multiplier = ResourceManager.Instance.GetProductionMultiplier(resourceType);
        int totalAmount = Mathf.RoundToInt(amountPerClick * multiplier);

        // Add resource
        ResourceManager.Instance.AddResource(resourceType, totalAmount);

        // Play effects
        StartCoroutine(PulseAnimation());

        // Play particles at click position
        if (clickParticlesPrefab != null && particlePool != null)
        {
            Vector3 clickPosition = GetClickWorldPosition();
            PlayParticleEffect(clickPosition);
        }
    }

    private void PlayClickSound()
    {
        if (clickSound == null || audioSource == null) return;

        // Apply pitch variation if enabled
        if (randomPitch)
        {
            audioSource.pitch = soundPitch * Random.Range(1f - pitchRandomRange, 1f + pitchRandomRange);
        }
        else
        {
            audioSource.pitch = soundPitch;
        }

        audioSource.PlayOneShot(clickSound, soundVolume);

        // Reset pitch for future sounds
        audioSource.pitch = 1f;
    }

    // Получаем мировые координаты клика на объекте
    private Vector3 GetClickWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        // Для 2D объектов: преобразуем позицию мыши в мировые координаты
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y,
            Mathf.Abs(Camera.main.transform.position.z))
        );

        // Добавляем смещение по Z для правильного отображения
        return new Vector3(worldPosition.x, worldPosition.y, transform.position.z + particleZOffset);
    }

    private IEnumerator PulseAnimation()
    {
        isAnimating = true;

        // Phase 1: Shrink and change color
        float elapsed = 0f;
        while (elapsed < pulseDuration / 2)
        {
            float progress = elapsed / (pulseDuration / 2);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * pulseScale, progress);
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(originalColor, pulseColor, progress);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 2: Restore size
        elapsed = 0f;
        while (elapsed < pulseDuration / 2)
        {
            float progress = elapsed / (pulseDuration / 2);
            transform.localScale = Vector3.Lerp(originalScale * pulseScale, originalScale, progress);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Phase 3: Restore color
        elapsed = 0f;
        while (elapsed < colorFadeDuration)
        {
            float progress = elapsed / colorFadeDuration;
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(pulseColor, originalColor, progress);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure we return to original state
        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        isAnimating = false;
    }

    // Обновлённый метод: спавнит партиклы в указанной позиции
    private void PlayParticleEffect(Vector3 position)
    {
        // Get particle system from pool
        ParticleSystem particleSystem = particlePool.GetParticleSystem();
        if (particleSystem == null) return;

        // Position at click location
        particleSystem.transform.position = position;

        // Configure particles
        var main = particleSystem.main;
        main.startColor = particleColor;
        main.startSize = Random.Range(0.1f, 0.3f) * particleSizeMultiplier;

        // Set particle count
        var emission = particleSystem.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, Random.Range(minParticles, maxParticles)));

        // Play and return to pool
        particleSystem.Play();
        particlePool.StartReturnTimer(particleSystem, main.duration);
    }

    // For debugging in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

public class ParticlePool : MonoBehaviour
{
    public static ParticlePool Instance;
    public ParticleSystem particlePrefab;
    public int initialPoolSize = 20;

    private ParticleSystem[] particlePool;
    private bool[] particlesInUse;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePool()
    {
        if (particlePrefab == null)
        {
            Debug.LogError("Cannot initialize pool: particlePrefab is null!");
            return;
        }

        particlePool = new ParticleSystem[initialPoolSize];
        particlesInUse = new bool[initialPoolSize];

        for (int i = 0; i < initialPoolSize; i++)
        {
            particlePool[i] = Instantiate(particlePrefab, transform);
            particlePool[i].gameObject.SetActive(false);
            particlesInUse[i] = false;
        }
    }

    public ParticleSystem GetParticleSystem()
    {
        // Ensure pool is initialized
        if (particlePool == null || particlePool.Length == 0)
        {
            if (particlePrefab == null)
            {
                Debug.LogError("Cannot get particles: Prefab missing!");
                return null;
            }
            InitializePool();
        }

        for (int i = 0; i < particlePool.Length; i++)
        {
            if (!particlesInUse[i])
            {
                particlesInUse[i] = true;
                particlePool[i].gameObject.SetActive(true);
                particlePool[i].Clear();
                return particlePool[i];
            }
        }

        // If all particles are in use, expand pool
        ExpandPool(5);
        return GetParticleSystem();
    }

    private void ExpandPool(int amount)
    {
        int oldSize = particlePool.Length;
        System.Array.Resize(ref particlePool, oldSize + amount);
        System.Array.Resize(ref particlesInUse, oldSize + amount);

        for (int i = oldSize; i < particlePool.Length; i++)
        {
            particlePool[i] = Instantiate(particlePrefab, transform);
            particlePool[i].gameObject.SetActive(false);
            particlesInUse[i] = false;
        }
    }

    public void ReturnParticleSystem(ParticleSystem particleSystem)
    {
        for (int i = 0; i < particlePool.Length; i++)
        {
            if (particlePool[i] == particleSystem)
            {
                particlesInUse[i] = false;
                particleSystem.gameObject.SetActive(false);
                return;
            }
        }
    }

    public void StartReturnTimer(ParticleSystem particleSystem, float duration)
    {
        StartCoroutine(ReturnAfterDelay(particleSystem, duration));
    }

    private IEnumerator ReturnAfterDelay(ParticleSystem particleSystem, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnParticleSystem(particleSystem);
    }
}