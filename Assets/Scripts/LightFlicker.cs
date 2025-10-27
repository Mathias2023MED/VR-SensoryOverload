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
    public float rampUpTime = 120f;   // Hvor lang tid der går før flicker starter
    public float flickerSpeed = 25f;  // Hvor hurtigt flickeret svinger
    public float flickerAmount = 0.25f; // Hvor kraftigt flickeret er

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
            // Før 120 sek: glid stille fra startIntensity til maxIntensity
            float t = time / rampUpTime;
            bloom.intensity.value = Mathf.Lerp(startIntensity, maxIntensity, t);
        }
        else
        {
            // Efter 120 sek: flicker omkring maxIntensity
            float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            float flicker = (noise - 0.5f) * 2f * flickerAmount; // mellem -amount og +amount
            bloom.intensity.value = Mathf.Clamp(maxIntensity + flicker, 0f, maxIntensity + flickerAmount);
        }
    }
}
