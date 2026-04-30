using UnityEngine;
using UnityEngine.AI;

public class VikingMovement : MonoBehaviour
{
    private Transform player;

    [Header("Settings")]
    public float attackDistance = 2f;
    public float runSpeed = 3.5f;

    [Header("Combat")]
    public float attackCooldown = 1.5f;
    public int axeDamage = 10;          // <--- NEW: He deals 10 damage!
    public float damageDelay = 0.5f;    // <--- NEW: Wait 0.5s for the axe to physically swing before dealing damage

    private float lastAttackTime;

    private Animator animator;
    private NavMeshAgent agent;

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

        if (distance > attackDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", 1f);
        }
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

                // NEW: Trigger the damage exactly when the axe swings down!
                Invoke("DealAxeDamage", damageDelay);
            }
        }
    }

    private void DealAxeDamage()
    {
        // 1. If the Viking died during the swing, cancel the attack!
        if (!this.enabled || player == null) return;

        // 2. Did the player run away before the axe landed? 
        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance <= attackDistance + 0.5f)
        {
            // 3. Player is still in range, deal 10 damage!
            PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(axeDamage);
            }
        }
    }
}