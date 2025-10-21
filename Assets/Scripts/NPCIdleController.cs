using UnityEngine;

public class NPCIdleController : MonoBehaviour
{
    public Animator animator;
    public int minIdleIndex = 1;
    public int maxIdleIndex = 5;
    public float idleSwitchInterval = 5f; // seconds between idle changes
    public bool randomizeSwitch = true;

    private float timer;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        // Start with a random idle
        int startIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
        animator.SetInteger("IdleIndex", startIdle);
    }

    void Update()
    {
        if (!randomizeSwitch) return; // For NPCs that stay in one idle

        timer += Time.deltaTime;
        if (timer >= idleSwitchInterval)
        {
            timer = 0f;
            int newIdle = Random.Range(minIdleIndex, maxIdleIndex + 1);
            animator.SetInteger("IdleIndex", newIdle);
        }
    }
}
