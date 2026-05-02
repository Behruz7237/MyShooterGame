using UnityEngine;
using UnityEngine.AI;

public class VikingMovement : MonoBehaviour
{
    private Transform player;

    [Header("Settings")]
    public float visionDistance = 20f;  // <--- NEW: How close before he notices you!
    public float attackDistance = 2f;
    public float runSpeed = 3.5f;

    [Header("Combat")]
    public float attackCooldown = 1.5f;
    public int axeDamage = 10;
    public float damageDelay = 0.5f;

    private float lastAttackTime;
    private Animator animator;
    private NavMeshAgent agent;
    private bool isAggro = false; // Keeps track of if he has seen the player yet

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = runSpeed;
        agent.stoppingDistance = attackDistance - 0.2f;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // 1. IS THE PLAYER FAR AWAY? (Stand Idle)
        if (distance > visionDistance && !isAggro)
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);
            return; // Stop running the rest of the Update code
        }

        // Once the player crosses the line, the Viking stays aggressive!
        isAggro = true;

        // 2. CHASE THE PLAYER
        if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", 1f);
        }
        // 3. ATTACK THE PLAYER
        else
        {
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
                Invoke("DealAxeDamage", damageDelay);
            }
        }
    }

    private void DealAxeDamage()
    {
        if (!this.enabled || player == null) return;

        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance <= attackDistance + 0.5f)
        {
            PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(axeDamage);
            }
        }
    }

    // Optional: Draw a visual circle in the Scene view so you know exactly where the village borders are!
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}