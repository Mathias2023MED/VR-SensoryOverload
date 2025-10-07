using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TunnelVisionUI : MonoBehaviour
{
    [Header("Volume Reference")]
    public Volume volume;

    [Header("Vignette Settings")]
    public float startIntensity = 0f;
    public float maxIntensity = 0.6f;
    public float startSmoothness = 0.3f;
    public float maxSmoothness = 1f;

    [Header("Blur Settings (Depth of Field)")]
    public float startBlur = 0f;
    public float maxBlur = 40f;

    [Header("Timing")]
    public float buildupTime = 10f;

    [Header("Pulse Effect")]
    public bool enablePulse = true;
    public float pulseSpeed = 2f;
    public float pulseAmplitude = 0.05f;

    private Vignette vignette;
    private DepthOfField dof;
    private float timer = 0f;
    private bool active = false;

    void Start()
    {
        if (volume == null)
        {
            Debug.LogError("TunnelVisionUI: Please assign a Volume!");
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
            dof.mode.value = DepthOfFieldMode.Gaussian;
            dof.gaussianStart.value = 0f;
            dof.gaussianEnd.value = startBlur;
            dof.active = false; // blur off until triggered
        }
    }

    void Update()
    {
        if (!active) return;

        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / buildupTime);

        // Animate vignette
        if (vignette != null)
        {
            float baseIntensity = Mathf.Lerp(startIntensity, maxIntensity, t);
            float pulse = 0f;

            if (enablePulse)
            {
                // pulse grows stronger as effect builds up
                float dynamicAmp = pulseAmplitude * t;
                pulse = Mathf.Sin(Time.time * pulseSpeed) * dynamicAmp;
            }

            vignette.intensity.value = Mathf.Clamp01(baseIntensity + pulse);
            vignette.smoothness.value = Mathf.Lerp(startSmoothness, maxSmoothness, t);
        }

        // Animate blur
        if (dof != null)
        {
            dof.active = true;
            dof.gaussianEnd.value = Mathf.Lerp(startBlur, maxBlur, t);
        }
    }

    public void StartTunnelVision()
    {
        timer = 0f;
        active = true;
    }

    public void ResetTunnelVision()
    {
        active = false;
        if (vignette != null)
        {
            vignette.intensity.value = startIntensity;
            vignette.smoothness.value = startSmoothness;
        }
        if (dof != null)
        {
            dof.active = false;
            dof.gaussianEnd.value = startBlur;
        }
    }
}
