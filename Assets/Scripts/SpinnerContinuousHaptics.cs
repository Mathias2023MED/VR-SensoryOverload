using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class SpinnerAlwaysAndResistanceHaptics : MonoBehaviour
{
    [Header("References")]
    public HapticImpulsePlayer rightHandHaptics;

    [Header("Haptics")]
    [Range(0f, 1f)]
    public float baseAmplitude = 0.15f;     // altid-aktiv vibration
    [Range(0f, 1f)]
    public float extraAmplitude = 0.45f;    // ekstra oveni ved rotation
    public float pulseDuration = 0.04f;     // længde på hver puls
    public float pulseInterval = 0.04f;     // hvor tit vi sender puls

    [Header("Hånd-rotation til modstand")]
    public float minAngularSpeed = 30f;     // under dette: kun baseAmplitude
    public float maxAngularSpeed = 720f;    // ved/over dette: fuld extraAmplitude

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
        hasLastRotation = false;
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        interactor = null;
        isHeld = false;
    }

    void Update()
    {
        if (!isHeld || rightHandHaptics == null)
            return;

        float amplitude = baseAmplitude;

        // Hvis vi har en hånd at måle på, læg ekstra oveni basen
        if (interactor != null)
        {
            Transform handTransform = interactor.transform;

            if (!hasLastRotation)
            {
                lastHandRotation = handTransform.rotation;
                hasLastRotation = true;
            }
            else
            {
                Quaternion currentRot = handTransform.rotation;
                Quaternion delta = currentRot * Quaternion.Inverse(lastHandRotation);
                delta.ToAngleAxis(out float angle, out _);
                float angularSpeed = angle / Time.deltaTime; // grader/sek

                lastHandRotation = currentRot;

                // Map hastighed -> 0..1
                float t = Mathf.InverseLerp(minAngularSpeed, maxAngularSpeed, angularSpeed);
                t = Mathf.Clamp01(t);

                // læg ekstra vibraton oveni basen
                amplitude += extraAmplitude * t;
            }
        }

        amplitude = Mathf.Clamp01(amplitude);

        // Send pulseret haptik hele tiden mens den er holdt
        if (Time.time - lastPulseTime >= pulseInterval)
        {
            rightHandHaptics.SendHapticImpulse(amplitude, pulseDuration);
            lastPulseTime = Time.time;
        }
    }
}
