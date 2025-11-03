using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TunnelVisionUI : MonoBehaviour
{
    [Header("Volume Reference")]
    public Volume volume; // will auto-find if not assigned

    [Header("Vignette")]
    public float startIntensity = 0f;
    public float maxIntensity = 0.6f;
    public float startSmoothness = 0.3f;
    public float maxSmoothness = 1f;

    [Header("Blur (Depth of Field)")]
    public float postBuildupSlightBlur = 3f; // pulses down to 0 after buildup
    public float gaussianStart = 0f;         // edge-to-center start for Gaussian
    public float blurFadeInTime = 2f;        // ease-in of blur amplitude after buildup

    [Header("Timing")]
    public float buildupTime = 150f;         // vignette buildup duration

    [Header("Pulse")]
    public bool enablePulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmplitude = 0.05f;     // vignette intensity oscillation

    private Vignette vignette;
    private DepthOfField dof;
    private float timer;

    void Awake()
    {
        // Auto-locate a Volume if not assigned
        if (volume == null)
            volume = GetComponent<Volume>() ?? FindObjectOfType<Volume>();
    }

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("TunnelVisionUI: No Volume found/assigned.");
            enabled = false;
            return;
        }

        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out dof);

        if (vignette != null)
        {
            vignette.intensity.value = startIntensity;
            vignette.smoothness.value = startSmoothness;
        }

        if (dof != null)
        {
            dof.mode.value = DepthOfFieldMode.Gaussian; // always Gaussian
            dof.gaussianStart.value = gaussianStart;
            dof.gaussianEnd.value = 0f;
            dof.active = false; // blur off during buildup
        }

        timer = 0f; // auto-start
    }

    void Update()
    {
        timer += Time.deltaTime;

        float t = (buildupTime <= 0f) ? 1f : Mathf.Clamp01(timer / buildupTime);
        bool afterBuildup = t >= 1f;

        // VIGNETTE
        if (vignette != null)
        {
            float baseIntensity = afterBuildup
                ? maxIntensity
                : Mathf.Lerp(startIntensity, maxIntensity, t);

            float baseSmoothness = afterBuildup
                ? maxSmoothness
                : Mathf.Lerp(startSmoothness, maxSmoothness, t);

            if (enablePulse)
            {
                float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
                baseIntensity = Mathf.Clamp01(baseIntensity + pulse);
            }

            vignette.intensity.value = baseIntensity;
            vignette.smoothness.value = baseSmoothness;
        }

        // BLUR (DoF)
        if (dof != null)
        {
            if (!afterBuildup)
            {
                dof.active = false;
                dof.gaussianEnd.value = 0f;
            }
            else
            {
                dof.active = true;

                float after = timer - buildupTime;
                float u = (blurFadeInTime <= 0f) ? 1f : Mathf.Clamp01(after / blurFadeInTime);
                u = Mathf.SmoothStep(0f, 1f, u); // ease-in of blur amplitude

                // Pulse in-phase with vignette: peak = no blur, trough = slight blur
                float sin01 = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
                float targetSlight = Mathf.Lerp(0f, postBuildupSlightBlur, u);
                float blurAmount = Mathf.Lerp(targetSlight, 0f, sin01);

                dof.gaussianStart.value = gaussianStart;
                dof.gaussianEnd.value = Mathf.Max(0f, blurAmount);
            }
        }
    }
}

