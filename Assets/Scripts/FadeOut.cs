using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOut : MonoBehaviour
{
    public Image panel; // Assign your black panel (UI Image) in the Inspector
    public float fadeDuration = 10f; // Duration in seconds

    private void Start()
    {
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        // Ensure panel starts fully black (opaque)
        Color color = panel.color;
        color.a = 1f;
        panel.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade alpha 1 → 0 smoothly
            color.a = 1f - t;
            panel.color = color;

            yield return null; // wait for next frame
        }

        // Fully transparent now
        color.a = 0f;
        panel.color = color;
    }
}
