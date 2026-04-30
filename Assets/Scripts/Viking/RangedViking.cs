using UnityEngine;
using UnityEngine.AI;

public class RangedViking : MonoBehaviour
{
    private Transform player;

    [Header("Movement")]
    public float throwDistance = 5f;
    public float runSpeed = 3.5f;

    [Header("Combat")]
    public GameObject spearPrefab;   // The flying spear
    public GameObject heldSpear;     // The 3D art spear in his hand
    public Transform throwPoint;     // Where the flying spear spawns

    public float throwCooldown = 3f;
    public float spearSpawnDelay = 0.2f; // Adjust this to match the exact throw frame!

    private float lastThrowTime;
    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.speed = runSpeed;
        agent.stoppingDistance = throwDistance;

        // Make sure he starts holding the spear!
        if (heldSpear != null) heldSpear.SetActive(true);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

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
        if (player == null) return;

        // 1. HIDE THE SPEAR IN HIS HAND!
        if (heldSpear != null) heldSpear.SetActive(false);

        // 2. Spawn the flying spear
        GameObject newSpear = Instantiate(spearPrefab, throwPoint.position, Quaternion.identity);

        Vector3 aimTarget = player.position + Vector3.up * 1.5f;
        newSpear.transform.LookAt(aimTarget);

        // Tilt the spear so the tip faces forward (use 90 or -90 depending on what worked for you!)
        newSpear.transform.Rotate(-90, 0, 0);

        // Launch it at a dodgeable speed
        newSpear.GetComponent<Rigidbody>().linearVelocity = newSpear.transform.up * 7f;

        // 3. RELOAD: Give him a new spear in his hand 1 second later!
        Invoke("ReloadSpear", 1f);
    }

    private void ReloadSpear()
    {
        // Make the hand spear visible again so he is ready for the next attack
        if (heldSpear != null) heldSpear.SetActive(true);
    }
}