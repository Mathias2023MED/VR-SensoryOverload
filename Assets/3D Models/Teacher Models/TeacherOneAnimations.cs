using UnityEngine;
using System.Collections;

public class TeacherIdleController : MonoBehaviour
{
    public Animator animator;
    public int minIdleIndex = 1;
    public int maxIdleIndex = 3;
    public bool randomizeSwitch = true;

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

        animator.speed = Random.Range(0.9f, 1.1f);

        if (!randomizeSwitch)
        {
            animator.Play($"Idle{startIdle}");
            enabled = false;
            return;
        }

        var info = animator.GetCurrentAnimatorStateInfo(0);
        clipLength = info.length;
        currentLoopDuration = clipLength * Random.Range(minLoopsBeforeSwitch, maxLoopsBeforeSwitch + 1);
    }

    void Update()
    {
        if (isInterrupted || !randomizeSwitch) return;

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

    public void TriggerInterruptSequence()
    {
        if (isInterrupted) return;
        isInterrupted = true;
        StartCoroutine(InterruptSequence());
    }

    private IEnumerator InterruptSequence()
    {
        // stop any current idle
        animator.SetInteger("IdleIndex", 0);
        animator.CrossFade("TalkToTeacher", 0.25f);

        // wait for that animation to finish (adjust to actual clip length)
        yield return new WaitForSeconds(3.5f);

        isInterrupted = false;
    }
}
