using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpinnerResistanceHaptics : MonoBehaviour
{
    [Header("References")]
    public HapticImpulsePlayer rightHandHaptics;

    [Header("Haptics")]
    public float maxAmplitude = 0.6f;      // max styrke
    public float pulseDuration = 0.04f;    // længde på hver “buzz”
    public float pulseInterval = 0.04f;    // hvor tit vi må sende haptics

    [Header("Hånd-rotation")]
    public float minAngularSpeed = 30f;    // under dette = ingen haptics
    public float maxAngularSpeed = 720f;   // ved/over dette = maxAmplitude

    private XRGrabInteractable grab;
    private IXRSelectInteractor interactor;
    private Quaternion lastHandRotation;
    private bool hasLastRotation = false;
    private bool isHeld = false;
    private float lastPulseTime = 0f;

    void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        grab.selectEntered.AddListener(OnSelectEntered);
        grab.selectExited.AddListener(OnSelectExited);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnSelectEntered);
        grab.selectExited.RemoveListener(OnSelectExited);
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        isHeld = true;
        hasLastRotation = false; // reset
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        interactor = null;
        isHeld = false;
    }

    void Update()
    {
        if (!isHeld || rightHandHaptics == null || interactor == null)
            return;

        Transform handTransform = interactor.transform;

        if (!hasLastRotation)
        {
            lastHandRotation = handTransform.rotation;
            hasLastRotation = true;
            return;
        }

        // Beregn hvor meget hånden har roteret siden sidste frame
        Quaternion currentRot = handTransform.rotation;
        Quaternion delta = currentRot * Quaternion.Inverse(lastHandRotation);
        delta.ToAngleAxis(out float angle, out _);   // vinkel i grader
        float angularSpeed = angle / Time.deltaTime; // grader/sekund
        lastHandRotation = currentRot;

        // Map hastighed -> intensitet (0..1)
        float t = Mathf.InverseLerp(minAngularSpeed, maxAngularSpeed, angularSpeed);
        float amplitude = Mathf.Clamp01(t) * maxAmplitude;

        if (amplitude <= 0f)
            return;

        // Send små pulseret haptik, så det føles som modstand
        if (Time.time - lastPulseTime >= pulseInterval)
        {
            rightHandHaptics.SendHapticImpulse(amplitude, pulseDuration);
            lastPulseTime = Time.time;
        }
    }
}
