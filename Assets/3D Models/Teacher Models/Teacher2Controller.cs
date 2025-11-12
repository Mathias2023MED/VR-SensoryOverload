using UnityEngine;
using System.Collections;
using System.Linq;

public class Teacher2Controller : MonoBehaviour
{
    [Header("References")]
    public Animator teacherAnimator;
    public Animator doorAnimator;

    [Header("Timing (in seconds)")]
    public float enterTime = 60f;   // When to start entering
    public float exitTime = 180f;   // When to trigger leaving

    private bool hasEntered = false;
    private bool isTalking = false;
    private bool hasLeft = false;

    private float startTime;

    void Start()
    {
        if (teacherAnimator == null)
            teacherAnimator = GetComponent<Animator>();

        startTime = Time.time;
    }

    void Update()
    {
        float elapsed = Time.time - startTime;
         
        // Trigger entrance at a specific time
        if (!hasEntered && elapsed  >= enterTime)
        {
            hasEntered = true;
            StartCoroutine(EnterSequence());
        }

        // Trigger leave at a specific time (or can be called externally)
        if (hasEntered && !hasLeft && elapsed >= exitTime)
        {
            hasLeft = true;
            StartCoroutine(LeaveSequence());
        }
    }

    private IEnumerator EnterSequence()
    {
        // Open the door and play the Enter animation
        doorAnimator.SetBool("isOpen", true);
        teacherAnimator.Play("Enter");

        // Wait one frame for Animator to register
        yield return null;

        float enterLength = teacherAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(enterLength);

        // Start looping Talk after Enter finishes
        teacherAnimator.Play("Talk");
        isTalking = true;
    }

    private IEnumerator LeaveSequence()
    {
        isTalking = false;

        // Close the door at the same time as Leave
        doorAnimator.SetBool("isOpen", false);
        teacherAnimator.Play("Leave");

        yield return null;

        float leaveLength = teacherAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(leaveLength);

        // End of sequence
        hasLeft = true;
    }
}
