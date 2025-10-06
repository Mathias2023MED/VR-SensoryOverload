using UnityEngine;

public class LocalSaturation : MonoBehaviour
{
    [Header("Farver")]
    public Color startColor = Color.white;   // Startfarve
    public Color endColor = Color.red;       // Slutfarve

    [Header("Indstillinger")]
    public float duration = 2f;              // Hvor lang tid farveskiftet skal tage (sekunder)
    private Renderer posterRenderer;
    private float timer = 0f;                // Tidsmåler

    void Start()
    {
        posterRenderer = GetComponent<Renderer>();
        if (posterRenderer == null)
        {
            Debug.LogError("Ingen Renderer fundet på objektet!");
            return;
        }

        // Brug instanced material, så andre objekter ikke påvirkes
        posterRenderer.material = new Material(posterRenderer.material);
        posterRenderer.material.color = startColor;
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration); // Normaliseret 0-1
            posterRenderer.material.color = Color.Lerp(startColor, endColor, t);
        }
    }
}
