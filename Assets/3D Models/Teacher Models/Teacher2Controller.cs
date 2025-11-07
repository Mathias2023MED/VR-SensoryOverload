using UnityEngine;
using System.Collections;

public class Teacher2Controller : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // --- Called from Time Script ---
    public void TriggerEntrance()
    {
        StartCoroutine(InterruptionSequence());
    }

    private IEnumerator InterruptionSequence()
    {
        animator.Play("Enter");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.Play("Talk");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        animator.Play("Leave");
    }
}
