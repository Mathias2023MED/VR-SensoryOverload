using UnityEngine;

public class FidgetSpinner : MonoBehaviour
{
    [Header("Spin Settings")]
    public float spinSpeed = 90f; // degrees per second
    private bool isSpinning = false;

    private void Update()
    {
        if (isSpinning)
        {
            // Rotate around Y-axis
            transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f);
        }
    }

    // Call this method to start spinning
    public void StartSpinning()
    {
        isSpinning = true;
    }

    // Optional: Call this to stop spinning
    public void StopSpinning()
    {
        isSpinning = false;
    }
}
