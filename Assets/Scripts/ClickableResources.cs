using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    public float particleZOffset = -0.1f;

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
    private Camera activeCamera; // Используем общее название вместо mainCamera

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Save original state
        originalScale = transform.localScale;
        originalColor = spriteRenderer.color;

        // Find and cache camera
        CacheActiveCamera();

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

    void Update()
    {
        // Проверяем камеру каждый кадр, если она была уничтожена
        if (activeCamera == null || !activeCamera.enabled || !activeCamera.gameObject.activeInHierarchy)
        {
            CacheActiveCamera();
        }
    }

    private void CacheActiveCamera()
    {
        // Поиск первой активной камеры в сцене
        Camera[] allCameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in allCameras)
        {
            if (cam.enabled && cam.gameObject.activeInHierarchy)
            {
                activeCamera = cam;
                Debug.Log($"Using camera: {cam.name}");
                return;
            }
        }

        // Если камеры не найдены
        Debug.LogError("No active cameras found in scene!");
        activeCamera = null;
    }

    private void InitializeAudio()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = soundVolume;
    }

    void OnMouseDown()
    {
        if (isAnimating || activeCamera == null) return;

        PlayClickSound();

        float multiplier = ResourceManager.Instance.GetProductionMultiplier(resourceType);
        int totalAmount = Mathf.RoundToInt(amountPerClick * multiplier);

        ResourceManager.Instance.AddResource(resourceType, totalAmount);

        StartCoroutine(PulseAnimation());

        if (clickParticlesPrefab != null && particlePool != null)
        {
            Vector3 clickPosition = GetClickWorldPosition();
            PlayParticleEffect(clickPosition);
        }
    }

    private void PlayClickSound()
    {
        if (clickSound == null || audioSource == null) return;

        if (randomPitch)
        {
            audioSource.pitch = soundPitch * Random.Range(1f - pitchRandomRange, 1f + pitchRandomRange);
        }
        else
        {
            audioSource.pitch = soundPitch;
        }

        audioSource.PlayOneShot(clickSound, soundVolume);
        audioSource.pitch = 1f;
    }

    private Vector3 GetClickWorldPosition()
    {
        if (activeCamera == null)
        {
            Debug.LogError("Active camera not available! Using object position.");
            return transform.position;
        }

        Vector3 mousePosition = Input.mousePosition;

        // Для 2D: используем ScreenToWorldPoint с глубиной = расстоянию до камеры
        float cameraDistance = Mathf.Abs(activeCamera.transform.position.z - transform.position.z);
        Vector3 worldPosition = activeCamera.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, cameraDistance)
        );

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

        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        isAnimating = false;
    }

    private void PlayParticleEffect(Vector3 position)
    {
        ParticleSystem particleSystem = particlePool.GetParticleSystem();
        if (particleSystem == null) return;

        particleSystem.transform.position = position;

        var main = particleSystem.main;
        main.startColor = particleColor;
        main.startSize = Random.Range(0.1f, 0.3f) * particleSizeMultiplier;

        var emission = particleSystem.emission;
        emission.SetBurst(0, new ParticleSystem.Burst(0f, Random.Range(minParticles, maxParticles)));

        particleSystem.Play();
        particlePool.StartReturnTimer(particleSystem, main.duration);
    }

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