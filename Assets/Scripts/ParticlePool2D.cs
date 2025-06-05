using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ParticlePool2D : MonoBehaviour
{
    public static ParticlePool2D Instance;
    public ParticleSystem particlePrefab;
    public int initialPoolSize = 20;

    private List<ParticleSystem> particlePool = new List<ParticleSystem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePool()
    {
        if (particlePrefab == null) return;

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateParticleInstance();
        }
    }

    private ParticleSystem CreateParticleInstance()
    {
        ParticleSystem ps = Instantiate(particlePrefab, transform);
        ps.gameObject.SetActive(false);
        particlePool.Add(ps);
        return ps;
    }

    public ParticleSystem GetParticleSystem()
    {
        foreach (ParticleSystem ps in particlePool)
        {
            if (!ps.gameObject.activeInHierarchy)
            {
                ps.gameObject.SetActive(true);
                ps.Clear();
                return ps;
            }
        }
        return CreateParticleInstance();
    }

    public void ReturnAfterDelay(ParticleSystem ps, float delay)
    {
        StartCoroutine(ReturnRoutine(ps, delay));
    }

    private IEnumerator ReturnRoutine(ParticleSystem ps, float delay)
    {
        yield return new WaitForSeconds(delay);
        ps.gameObject.SetActive(false);
    }
}