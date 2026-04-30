using UnityEngine;
using UnityEngine.AI;

public class RangedViking : MonoBehaviour
{
    private Transform player;

    [Header("Movement")]
    public float throwDistance = 5f; // Stops 5 meters away!
    public float runSpeed = 3f;

    [Header("Combat")]
    public GameObject spearPrefab;
    public Transform throwPoint; // The spot near his hand where the spear spawns
    public float throwCooldown = 3f; // Waits 3 seconds between throws
    public float spearSpawnDelay = 0.5f; // Wait half a second for the animation hand to swing forward

    private float lastThrowTime;
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = runSpeed;
        agent.stoppingDistance = throwDistance;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > throwDistance)
        {
            // 1. CHASE
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // 2. IN RANGE - STOP AND THROW
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            // Turn to face the player
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 8f);
            }

            // Attack cooldown check
            if (Time.time >= lastThrowTime + throwCooldown)
            {
                // Play the animation
                animator.SetTrigger("Throw");
                lastThrowTime = Time.time;

                // Spawn the spear a split-second later so it matches the hand movement!
                Invoke("SpawnSpear", spearSpawnDelay);
            }
        }
    }

    private void SpawnSpear()
    {
        if (player == null) return;

        // 1. Spawn the single spear object
        GameObject newSpear = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);

        // 2. Aim at the player's chest
        Vector3 aimTarget = player.position + Vector3.up * 1.5f;
        newSpear.transform.LookAt(aimTarget);

        // 3. THE MAGIC FIX: Bend the spear 90 degrees forward so the tip points at you!
        newSpear.transform.Rotate(-90, 0, 0);

        // 4. Push it forward incredibly fast (15 speed)
        newSpear.GetComponent<Rigidbody>().linearVelocity = newSpear.transform.up * 5f;
    }
}