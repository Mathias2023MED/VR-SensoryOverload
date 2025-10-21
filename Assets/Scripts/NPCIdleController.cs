using UnityEngine;
using System.Collections;

public class NPCIdleController : MonoBehaviour
{
    public Animator animator;
    public int minIdleIndex = 1;
    public int maxIdleIndex = 5;
    public bool randomizeSwitch = true;

    [Header("Timing Settings")]
    public int minLoopsBeforeSwitch = 2;
    public int maxLoopsBeforeSwitch = 4;

    private float clipLength;
    private float timer;
    private float currentLoopDuration;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);

        animator.speed = Random.Range(0.9f, 1.1f);

        // If random switching is disabled, just keep looping one idle forever
        if (!randomizeSwitch)
        {
            animator.Play($"Idle{startIdle}");
            enabled = false; // no need to update
            return;
        }

        // Otherwise set up normal switching logic
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }

    void Update()
    {
        if (!randomizeSwitch) return;

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

        // wait one frame so Unity updates the animator state
        yield return null;

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }
}
