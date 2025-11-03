using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image panel;
    public float fadeDuration = 10f;
    public float blackHoldDuration = 2f; // How long it stays black before fading

    private void Awake()
    {
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        // Panel starts fully opaque
        Color color = panel.color;
        color.a = 1f;
        panel.color = color;

        // Start with AudioListener muted
        AudioListener.volume = 0f;

        // Hold black for a moment
        yield return new WaitForSeconds(blackHoldDuration);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // independent of time scale
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Smooth fade for panel
            color.a = Mathf.SmoothStep(1f, 0f, t);
            panel.color = color;

            // Smooth fade for audio
            AudioListener.volume = Mathf.SmoothStep(0f, 1f, t);

            yield return null;
        }

        // Ensure fully transparent & full volume
        color.a = 0f;
        panel.color = color;
        AudioListener.volume = 1f;
    }
}
