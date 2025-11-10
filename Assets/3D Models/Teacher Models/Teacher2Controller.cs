using UnityEngine;
using System.Collections;
using System.Linq;

public class Teacher2Controller : MonoBehaviour
{
    [Header("References")]
    public Animator teacherAnimator;   // Animator on Teacher2
    public Animator doorAnimator;      // Animator on the door

    [Header("Timing (in seconds)")]
    public float enterTime = 60f;      // When to start "Enter" sequence
    public float talkDuration = 120f;  // How long the talk lasts before "Leave"
    public float exitTime = 180f;      // Optional override if you want a fixed exit moment

    private bool sequenceStarted = false;
    private bool hasLeft = false;

    void Start()
    {
        if (teacherAnimator == null)
            teacherAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        // Trigger entrance based on timing
        if (!sequenceStarted && Time.time >= enterTime)
        {
            sequenceStarted = true;
            StartCoroutine(EntranceSequence());
        }

        // Force leave if exitTime reached
        if (sequenceStarted && !hasLeft && Time.time >= exitTime)
        {
            StopAllCoroutines();
            StartCoroutine(LeaveSequence());
        }
    }

    private IEnumerator EntranceSequence()
    {
        // Open door and play enter animation
        doorAnimator.SetBool("isOpen", true);
        teacherAnimator.Play("Enter");

        // Wait one frame for Animator to update state
        yield return null;

        // Get the length of the Enter clip safely
        float enterLength = teacherAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        // Wait for Enter animation to finish
        yield return new WaitForSeconds(enterLength);

        // Start talking animation
        teacherAnimator.Play("Talk");

        // Wait for talkDuration or until interrupted
        yield return new WaitForSeconds(talkDuration);

        // Trigger leave sequence
        StartCoroutine(LeaveSequence());
    }

    private IEnumerator LeaveSequence()
    {
        hasLeft = true;

        // Interrupt talk and play Leave
        teacherAnimator.Play("Leave");

        // Wait one frame for Animator to update
        yield return null;

        // Get length of Leave animation
        float leaveLength = teacherAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

        yield return new WaitForSeconds(leaveLength);

        // Close the door after leaving
        doorAnimator.SetBool("isOpen", false);
    }
}
