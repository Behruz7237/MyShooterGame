using UnityEngine;

public class MutantBossController : MonoBehaviour
{
    [Header("Boss Health")]
    public int maxHealth = 500;
    private int currentHealth;
    private bool isDead = false;

    [Header("Combat Settings")]
    public float punchDistance = 4f;    // How close you must be for him to punch
    public float punchCooldown = 2.5f;  // Seconds between punches
    public int punchDamage = 25;        // How much health he takes from you
    public float damageDelay = 0.6f;    // Delay to match the physical punch swinging

    private float lastPunchTime;
    private Animator animator;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        // Find the player automatically
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isDead || player == null) return;

        // How far is the player?
        float distance = Vector3.Distance(transform.position, player.position);

        // If the player gets too close, PUNCH!
        if (distance <= punchDistance)
        {
            // Turn to face the player while punching
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            // Check if enough time has passed since the last punch
            if (Time.time >= lastPunchTime + punchCooldown)
            {
                animator.SetTrigger("Punch");
                lastPunchTime = Time.time;

                // Deal damage exactly when the fist hits the player
                Invoke("DealPunchDamage", damageDelay);
            }
        }
    }

    private void DealPunchDamage()
    {
        if (isDead || player == null) return;

        // Make sure the player didn't dodge away!
        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance <= punchDistance + 1f)
        {
            PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(punchDamage);
            }
        }
    }

    // Your bullets will call this function!
    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("Boss took damage! HP left: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // Play the death animation
        animator.SetTrigger("Die");

        // Turn off his collider so the player can walk over his giant body
        Collider bossCollider = GetComponent<Collider>();
        if (bossCollider != null) bossCollider.enabled = false;

        // Destroy his body after 10 seconds to free up memory
        Destroy(gameObject, 10f);

        Debug.Log("YOU DEFEATED THE BOSS!");
        // (If you have a WinManager, you can call it here!)
    }

    // Draws a red circle to show his punch range in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchDistance);
    }
}