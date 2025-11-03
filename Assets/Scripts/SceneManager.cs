using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // for SceneManager.LoadScene
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public Image panel; // Assign your black panel (UI Image) in the Inspector
    public int nextSceneIndex = 1;
    public float fadeDuration = 5f; // 5 seconds

    private void Start()
    {
        FadeToBlack();
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeOutAndChangeScene());
    }

    private IEnumerator FadeOutAndChangeScene()
    {
        // Ensure panel starts fully transparent
        Color color = panel.color;
        color.a = 0f;
        panel.color = color;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);

            // Fade alpha 0 → 1 smoothly
            color.a = t;
            panel.color = color;

            yield return null; // wait for next frame
        }

        // Fully black now — change the scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneIndex);
    }
}
