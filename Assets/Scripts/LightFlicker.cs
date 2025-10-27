using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class LightFlicker : MonoBehaviour
{
     [Header("Bloom Settings")]
    public Volume volume;
    public float startIntensity = 0.2f;
    public float maxIntensity = 1.2f;
    public float scatter = 0.6f;

    [Header("Timing")]
    public float rampUpTime = 120f;     // Hvor lang tid til den når max
    public float pulseSpeed = 0.5f;     // Hvor hurtigt den “ånder”
    public float pulseAmount = 0.2f;    // Hvor meget den varierer (op/ned fra max)

    private Bloom bloom;

    void Start()
    {
        if (volume.profile.TryGet(out bloom))
        {
            bloom.intensity.value = startIntensity;
            bloom.scatter.value = scatter;
        }
        else
        {
            Debug.LogError("Bloom not found in Volume Profile!");
        }
    }

    void Update()
    {
        if (bloom == null) return;

        float time = Time.time;

        if (time < rampUpTime)
        {
            // Før 120 sekunder: jævn stigning mod max
            float t = time / rampUpTime;
            bloom.intensity.value = Mathf.Lerp(startIntensity, maxIntensity, t);
        }
        else
        {
            // Efter 120 sekunder: smooth “blussen” omkring maxIntensity
            float pulse = (Mathf.Sin((time - rampUpTime) * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
            float flickerValue = Mathf.Lerp(maxIntensity - pulseAmount, maxIntensity + pulseAmount, pulse);

            bloom.intensity.value = flickerValue;
        }
    }
}
