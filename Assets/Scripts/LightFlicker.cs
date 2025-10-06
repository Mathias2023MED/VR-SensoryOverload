using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class LightFlicker : MonoBehaviour
{
    [Header("Bloom Settings")]
    public Volume volume;                 // Volume med Bloom aktiveret
    public float intensityMin = 0.3f;     // Minimum intensity
    public float intensityMax = 0.6f;     // Maximum intensity
    public float scatterMin = 0.5f;       // Minimum scatter
    public float scatterMax = 0.7f;       // Maximum scatter
    public float duration = 2f;           // Hvor lang tid en fuld pulsering tager (i sekunder)

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

        // Beregn hvor langt vi er i pulsen (0-1)
        float t = (Time.time % duration) / duration;

        // Brug en sin-lignende kurve for smooth pulsering
        float pulse = (Mathf.Sin(t * Mathf.PI * 2f - Mathf.PI / 2f) + 1f) / 2f;
        // t=0 -> pulse=0, t=duration/2 -> pulse=1, t=duration -> pulse=0

        // Sæt Bloom værdier
        bloom.intensity.value = Mathf.Lerp(intensityMin, intensityMax, pulse);
        bloom.scatter.value = Mathf.Lerp(scatterMin, scatterMax, pulse);
    }
}
