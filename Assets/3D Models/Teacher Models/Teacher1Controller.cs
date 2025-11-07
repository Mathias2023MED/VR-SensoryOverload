using UnityEngine;
using System.Collections;

public class Teacher1Controller : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Idle Settings")]
    public int minIdleIndex = 1;
    public int maxIdleIndex = 2;
    public int minLoopsBeforeSwitch = 2;
    public int maxLoopsBeforeSwitch = 4;

    private float clipLength;
    private float timer;
    private float currentLoopDuration;

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();

        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
        animator.speed = Random.Range(0.9f, 1.1f);

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);

        // Teacher always talking while teaching
        animator.SetBool("IsTalking", true);
    }

    void Update()
    {
        // If currently in interruption, stop idle switching
        if (animator.GetBool("IsInterrupted"))
            return;

        timer += Time.deltaTime;
        if (timer >= currentLoopDuration)
        {
            timer = 0f;
            StartCoroutine(SwitchIdle());
        }
    }

    private IEnumerator SwitchIdle()
    {
        int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", newIdle);

        yield return null;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }

    // ?? Called by the Time Script when interruption begins
    public void StartInterruption()
    {
        animator.SetBool("IsInterrupted", true);
        animator.SetBool("IsTalking", false); // stop mouth loop
    }

    // ?? Called by the Time Script when interruption ends
    public void EndInterruption()
    {
        animator.SetBool("IsInterrupted", false);
        animator.SetBool("IsTalking", true); // resume talking loop
    }
}
