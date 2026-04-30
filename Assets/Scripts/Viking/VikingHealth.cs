using UnityEngine;
using UnityEngine.AI;

public class VikingHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;

    private Animator animator;
    private NavMeshAgent agent;
    private Collider vikingCollider;
    private VikingMovement movementScript;

    private bool isDead = false;

    void Start()
    {
        // Start with full health
        currentHealth = maxHealth;

        // Grab all the components we need to turn off when he dies
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        vikingCollider = GetComponent<Collider>();
        movementScript = GetComponent<VikingMovement>();
    }

    // Your Gun script will call this function when a bullet hits the Viking
    public void TakeDamage(int damageAmount)
    {
        // If he's already dead, don't take more damage
        if (isDead) return;

        currentHealth -= damageAmount;
        Debug.Log("Viking hit! Remaining HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Viking Died!");

        // 1. Play the death animation
        animator.SetTrigger("Die");

        // 2. Turn off the NavMesh and Movement script so he stops chasing you
        if (agent != null) agent.enabled = false;
        if (movementScript != null) movementScript.enabled = false;

        // 3. Turn off the Collider so you can walk over his dead body 
        // (and so bullets don't hit the invisible ghost of the Viking)
        if (vikingCollider != null) vikingCollider.enabled = false;

        // 4. Destroy the GameObject after 5 seconds to keep the game running smoothly
        Destroy(gameObject, 5f);
    }
}