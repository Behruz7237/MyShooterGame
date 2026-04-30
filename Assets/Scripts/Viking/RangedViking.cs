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
        // 1. IF HE IS DEAD (SCRIPT WAS TURNED OFF), STOP IMMEDIATELY!
        if (!this.enabled) return;

        if (player == null) return;

        // 1. Hide the hand spear
        if (heldSpear != null) heldSpear.SetActive(false);

        // 2. Calculate exactly where we want to throw it
        Vector3 aimTarget = player.position + Vector3.up * 1.5f;

        // 3. Calculate the direction from the hand to the player
        Vector3 throwDirection = (aimTarget - throwPoint.position).normalized;

        // 4. Spawn the spear already looking in the exact right direction
        GameObject newSpear = Instantiate(spearPrefab, throwPoint.position, Quaternion.LookRotation(throwDirection));

        // 5. Fix the tilt of the art so the sharp end points forward. 
        // Try 90 if -90 points it backward!
        newSpear.transform.Rotate(90, 0, 0);

        // 6. Push it directly along the throwDirection we calculated earlier!
        newSpear.GetComponent<Rigidbody>().linearVelocity = throwDirection * 7f;

        // 7. Reload the hand spear
        Invoke("ReloadSpear", 1f);
    }

    private void ReloadSpear()
    {
        // IF HE DIED WHILE WAITING TO RELOAD, DO NOTHING!
        if (!this.enabled) return;

        // Make the hand spear visible again so he is ready for the next attack
        if (heldSpear != null) heldSpear.SetActive(true);
    }
}