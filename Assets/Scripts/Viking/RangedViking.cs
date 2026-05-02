using UnityEngine;
using UnityEngine.AI;

public class RangedViking : MonoBehaviour
{
    private Transform player;

    [Header("Movement")]
    public float visionDistance = 20f;  // <--- NEW: How close before he notices you!
    public float throwDistance = 10f;   // How close he gets before throwing
    public float runSpeed = 3.5f;

    [Header("Combat")]
    public GameObject spearPrefab;   // The flying spear
    public GameObject heldSpear;     // The 3D art spear in his hand
    public Transform throwPoint;     // Where the flying spear spawns

    public float throwCooldown = 3f;
    public float spearSpawnDelay = 0.2f;

    private float lastThrowTime;
    private Animator animator;
    private NavMeshAgent agent;
    private bool isAggro = false; // <--- NEW: Keeps track of if he has seen the player

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = runSpeed;
        agent.stoppingDistance = throwDistance;

        if (heldSpear != null) heldSpear.SetActive(true);

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
            return;
        }

        // Player crossed the line! Wake up and stay mad!
        isAggro = true;

        if (distance > throwDistance)
        {
            // Chase
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            // In Range - Stop
            agent.isStopped = true;
            animator.SetFloat("Speed", 0f);

            // Look at player
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 8f);
            }

            // Throw Cooldown
            if (Time.time >= lastThrowTime + throwCooldown)
            {
                animator.SetTrigger("Throw");
                lastThrowTime = Time.time;
                Invoke("SpawnSpear", spearSpawnDelay);
            }
        }
    }

    private void SpawnSpear()
    {
        if (!this.enabled) return;
        if (player == null) return;

        if (heldSpear != null) heldSpear.SetActive(false);

        Vector3 aimTarget = player.position + Vector3.up * 1.5f;
        Vector3 throwDirection = (aimTarget - throwPoint.position).normalized;

        GameObject newSpear = Instantiate(spearPrefab, throwPoint.position, Quaternion.LookRotation(throwDirection));
        newSpear.transform.Rotate(90, 0, 0);
        newSpear.GetComponent<Rigidbody>().linearVelocity = throwDirection * 15f; // Adjust spear flight speed here!

        Invoke("ReloadSpear", 1f);
    }

    private void ReloadSpear()
    {
        if (!this.enabled) return;
        if (heldSpear != null) heldSpear.SetActive(true);
    }

    // Draw the Vision (Yellow) and Throw (Red) circles in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, throwDistance);
    }
}