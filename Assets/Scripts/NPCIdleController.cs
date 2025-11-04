using UnityEngine;
using System.Collections;

public enum TalkDirection { None, Left, Right }

public class NPCIdleController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Idle Settings")]
    public int minIdleIndex = 1;
    public int maxIdleIndex = 5;
    public bool randomizeSwitch = true;

    [Header("Timing Settings")]
    public int minLoopsBeforeSwitch = 2;
    public int maxLoopsBeforeSwitch = 4;

    [Header("Talking Settings")]
    public bool canTalk = false;
    public TalkDirection talkDirection = TalkDirection.None;

    private float clipLength;
    private float timer;
    private float currentLoopDuration;
    private bool isTalking = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
        animator.speed = Random.Range(0.9f, 1.1f);

        // If random switching is disabled, just loop one idle forever
        if (!randomizeSwitch)
        {
            animator.Play($"Idle{startIdle}");
            return;
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }

    void Update()
    {
        // Do not switch idles if talking
        if (isTalking || !randomizeSwitch) return;

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

    // === TALKING METHODS ===

    public void TriggerTalking()
    {
        if (!canTalk || isTalking) return;

        isTalking = true;

        // pick correct talk animation
        string animName = talkDirection == TalkDirection.Left ? "TalkLeft" : "TalkRight";
        animator.CrossFade(animName, 0.25f);

        // optional: turn on mouth movement layer
        animator.SetBool("IsTalking", true);
    }

    public void StopTalking()
    {
        if (!isTalking) return;

        isTalking = false;
        animator.SetBool("IsTalking", false);

        // resume idle switching
        int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", newIdle);
        animator.CrossFade($"Idle{newIdle}", 0.3f);
    }
}
