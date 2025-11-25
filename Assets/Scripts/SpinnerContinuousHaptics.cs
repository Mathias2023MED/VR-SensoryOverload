using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpinnerContinuousHaptics : MonoBehaviour
{
    public HapticImpulsePlayer rightHandHaptics;

    public float amplitude = 0.2f;
    public float duration = 0.05f;
    public float pulseInterval = 0.05f;

    private XRGrabInteractable grab;
    private bool isHeld = false;
    private float lastPulseTime = 0f;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        isHeld = true;
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        isHeld = false;
    }

    void Update()
    {
        if (!isHeld || rightHandHaptics == null)
            return;

        if (Time.time - lastPulseTime >= pulseInterval)
        {
            rightHandHaptics.SendHapticImpulse(amplitude, duration);
            lastPulseTime = Time.time;
        }
    }
}


