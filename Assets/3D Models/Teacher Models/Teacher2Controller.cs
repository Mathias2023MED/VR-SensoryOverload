using UnityEngine;
using System.Collections;

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

        if (doorAnimator != null)
        doorAnimator.SetBool("isOpen", false); // ensure door starts closed
    }

    void Update()
    {
        // Check if it's time to start the entrance
        if (!sequenceStarted && Time.time >= enterTime)
        {
            sequenceStarted = true;
            StartCoroutine(EntranceSequence());
        }

        // Allow the Leave animation to override Talk early if needed
        if (sequenceStarted && !hasLeft && Time.time >= exitTime)
        {
            StopAllCoroutines();
            StartCoroutine(LeaveSequence());
        }
    }

    private IEnumerator EntranceSequence()
    {
        // Open door and play enter
        doorAnimator.SetBool("isOpen", true);
        teacherAnimator.Play("Enter");

        // Wait for enter animation to finish
        yield return new WaitForSeconds(teacherAnimator.GetCurrentAnimatorStateInfo(0).length);

        // Start talking animation
        teacherAnimator.Play("Talk");

        // Wait until talk duration ends or until overridden
        yield return new WaitForSeconds(talkDuration);

        // Proceed to leave
        StartCoroutine(LeaveSequence());
    }

    private IEnumerator LeaveSequence()
    {
        hasLeft = true;

        // Interrupt talk, play leave animation
        teacherAnimator.Play("Leave");

        // Wait for the animation to finish before closing door
        yield return new WaitForSeconds(teacherAnimator.GetCurrentAnimatorStateInfo(0).length);

        doorAnimator.SetBool("isOpen", false);
    }
}
