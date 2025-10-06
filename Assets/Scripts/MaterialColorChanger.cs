using UnityEngine;
/// <summary>
///You assign a material from your project (targetMaterial) and the target color (endColor) in the Inspector.
///The script finds the material on the GameObject’s Renderer by comparing names.
///It creates a unique instance of that material so only this object is affected, not others using the same material.
///A timer counts up in Update() over time.
///The script interpolates (Lerp) the color from the original color to the target color based on how much time has passed.
///Only the specified material is updated, leaving all other materials on the object unchanged.
/// </summary>
public class MaterialColorChanger : MonoBehaviour
{
    [Header("Materiale til farveskift")]
    public Material targetMaterial;
    public Color endColor = Color.red; // The color the material should change to
    public float duration = 2f; //How long the color change should take in seconds

    private Color startColor; // Stores the starting color of the material
    private float timer = 0f; // Tracks how much time has passed since the color change started
    private Renderer objRenderer; // The Renderer component on the GameObject
    private int materialIndex = -1; // Index of the target material in the Renderer.materials array. -1 means "not found"

    void Start()
    {
        // Get the Renderer component from this GameObject
        objRenderer = GetComponent<Renderer>();
        if (objRenderer == null)
        {
            Debug.LogError("Renderer not found");
            return;
        }
        // Check if a material has been assigned in the Inspector
        if (targetMaterial == null)
        {
            Debug.LogError("No material chosen");
            return;
        }

        Material[] mats = objRenderer.materials; // Get all materials assigned to the Renderer

        
        for (int i = 0; i < mats.Length; i++) // Find the material by name in the Renderer
        {
            if (mats[i].name.Contains(targetMaterial.name)) // Compare material names because Renderer.materials contains instances
            {
                materialIndex = i; // Save the index of the target material
                mats[i] = new Material(mats[i]); // Create a new instance of this material so only this object is affected
                startColor = mats[i].color; // Store the starting color of this material
                objRenderer.materials = mats; // Assign the modified array back to the Renderer
                break; // Stop looping once the material is found
            }
        }

        if (materialIndex == -1) // If the material was not found in the Renderer
        {
            Debug.LogWarning("Material not found");
        }
    }

    void Update()
    {
        if (materialIndex == -1) return; // Stop if material not found

        // Gradually change the color over time
        if (timer < duration)
        {
            timer += Time.deltaTime; // Increase timer by the time passed since last frame
            float t = Mathf.Clamp01(timer / duration); // Normalize timer to a 0-1 range

            Material[] mats = objRenderer.materials; // Get current materials from the Renderer
            mats[materialIndex].color = Color.Lerp(startColor, endColor, t); // Linearly interpolate color
        }
    }
}
 