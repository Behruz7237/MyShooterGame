using UnityEngine;
using UnityEngine.AI;

public class VikingMovement : MonoBehaviour
{
    private Transform player;

    [Header("Settings")]
    public float attackDistance = 2f;
    public float runSpeed = 3.5f;

    [Header("Combat")]
    public float attackCooldown = 1.5f; // How many seconds between each axe swing?
    private float lastAttackTime;       // Remembers the exact time he last swung

    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = runSpeed;
        agent.stoppingDistance = attackDistance - 0.2f;

        // Automatically find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Viking cannot find the Player! Make sure your Player is tagged as 'Player'.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackDistance)
        {
            // 1. CHASE THE PLAYER
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // 2. IN RANGE (STAND STILL AND FACE PLAYER)
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            // Turn to face you
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;

            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            // --- THE "WEIRD LOOP" FIX ---
            // Only attack IF enough time has passed since the last attack
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");

                // Reset the timer so he has to wait again before the next swing!
                lastAttackTime = Time.time;
            }
        }
    }
}