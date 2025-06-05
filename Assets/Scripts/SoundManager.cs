using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private int maxConcurrentSounds = 5;

    [Header("Resource Sound Defaults")]
    [SerializeField] private AudioClip defaultResourceSound;
    [SerializeField] private float defaultVolume = 0.7f;
    [SerializeField] private float defaultPitch = 1f;

    private Queue<AudioSource> audioSourcePool = new Queue<AudioSource>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        // Create main SFX source if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 0f;
        }

        // Create pool of additional audio sources
        for (int i = 0; i < maxConcurrentSounds; i++)
        {
            CreateAudioSourceInPool();
        }
    }

    private AudioSource CreateAudioSourceInPool()
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = defaultVolume;
        audioSourcePool.Enqueue(source);
        return source;
    }

    public void PlayResourceSound(
        AudioClip clip,
        ResourceType resourceType,
        float volume = -1f,
        float pitch = -1f,
        bool randomPitch = true,
        float pitchRange = 0.1f)
    {
        if (clip == null) clip = defaultResourceSound;
        if (clip == null) return;

        // Get audio source from pool
        AudioSource source = GetAvailableAudioSource();

        // Apply parameters
        source.clip = clip;
        source.volume = (volume >= 0) ? volume : defaultVolume;

        // Handle pitch
        if (pitch < 0) pitch = defaultPitch;
        if (randomPitch)
        {
            source.pitch = pitch * Random.Range(1f - pitchRange, 1f + pitchRange);
        }
        else
        {
            source.pitch = pitch;
        }

        // Play and return to pool
        source.Play();
        StartCoroutine(ReturnToPoolAfterPlay(source));
    }

    private AudioSource GetAvailableAudioSource()
    {
        // Check for available sources
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // Create new if all busy
        Debug.LogWarning("Sound pool exhausted! Creating new AudioSource.");
        return CreateAudioSourceInPool();
    }

    private IEnumerator ReturnToPoolAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        // Reset settings
        source.volume = defaultVolume;
        source.pitch = 1f;
    }
}