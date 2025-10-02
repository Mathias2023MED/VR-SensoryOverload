using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class LightFlicker : MonoBehaviour
{
    [Header("Bloom Settings")]
    public Volume volume;                 // Volume med Bloom aktiveret
    public float intensityMin = 0.3f;     // Minimum intensity
    public float intensityMax = 0.6f;       // Maximum intensity
    public float scatterMin = 0.5f;       // Minimum scatter
    public float scatterMax = 0.7f;       // Maximum scatter
    public float speed = 1f;              // Hvor hurtigt det pulserer

    private Bloom bloom;

    void Start()
    {
        // Find Bloom i volume-profilen
        if (volume.profile.TryGet<Bloom>(out bloom))
        {
            // Start med min værdier
            bloom.intensity.value = intensityMin;
            bloom.scatter.value = scatterMin;
        }
        else
        {
            Debug.LogError("Bloom not found in Volume Profile!");
        }
    }

    void Update()
    {
        if (bloom == null) return;

        // Pulserende effekt mellem min og max
        bloom.intensity.value = Mathf.Lerp(intensityMin, intensityMax, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        bloom.scatter.value = Mathf.Lerp(scatterMin, scatterMax, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
    }
}
