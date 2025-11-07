using UnityEngine;
using System.Collections;

public class Teacher1Controller : MonoBehaviour
{
    public Animator animator;
    public int minIdleIndex = 1;
    public int maxIdleIndex = 3;
    public int minLoopsBeforeSwitch = 2;
    public int maxLoopsBeforeSwitch = 4;

    private float clipLength;
    private float timer;
    private float currentLoopDuration;
    private bool isInterrupted = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
        animator.Play($"Idle{startIdle}");

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }

    void Update()
    {
        if (isInterrupted) return;

        timer += Time.deltaTime;
        if (timer >= currentLoopDuration)
        {
            timer = 0f;
            int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
            animator.SetInteger("IdleIndex", newIdle);

            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            clipLength = info.length;
            currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
        }
    }

    // --- Called externally by Time Script ---
    public void TriggerInterruption()
    {
        StartCoroutine(HandleInterruption());
    }

    private IEnumerator HandleInterruption()
    {
        isInterrupted = true;
        animator.SetBool("isTalkingInterrupted", true);
        animator.Play("TalkInterrupted");

        yield return new WaitForSeconds(5f); // length of interaction

        animator.SetBool("isTalkingInterrupted", false);
        isInterrupted = false;
    }
}
