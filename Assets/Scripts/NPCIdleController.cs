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
    public bool canTalk = false;           // normal talking students
    public bool isSpecialStudent = false;  // special student with unique mouth animation
    public float talkTime = 60f;           // when to start talking (seconds since scene start)
    public float talkDuration = -1f;       // duration; -1 = infinite
    public TalkDirection talkDirection = TalkDirection.None;

    // Internal variables
    private float clipLength;
    private float timer;
    private float currentLoopDuration;
    private bool isTalking = false;
    private float sceneTimer = 0f;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Start on a random idle
        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
        animator.speed = Random.Range(0.9f, 1.1f);

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
        // Track scene time
        sceneTimer += Time.deltaTime;

        // Auto-trigger talking by scene time
        if (canTalk && !isTalking && sceneTimer >= talkTime)
        {
            Debug.Log($"{name} starting talk at {sceneTimer}s");
            TriggerTalking();
        }

        // Idle switching logic
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

        // Body talk animation (directional)
        string animName = TalkDirection.None switch
        {
            TalkDirection.Left => "TalkLeft",
            TalkDirection.Right => "TalkRight",
            _ => "TalkLeft" // default
        };
        animator.CrossFade(animName, 0.25f);

        // Mouth animation layer
        if (isSpecialStudent)
            animator.SetBool("IsSpecialTalking", true);
        else
            animator.SetBool("IsTalking", true);

        // Optional: stop after duration if > 0
        if (talkDuration > 0f)
            StartCoroutine(StopTalkingAfterDelay(talkDuration));
    }

    private IEnumerator StopTalkingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StopTalking();
    }

    public void StopTalking()
    {
        if (!isTalking) return;

        isTalking = false;

        // Reset mouth layer
        animator.SetBool("IsTalking", false);
        animator.SetBool("IsSpecialTalking", false);

        // Resume random idle switching
        int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", newIdle);
        animator.CrossFade($"Idle{newIdle}", 0.3f);
    }
}
