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

    // We check for BOTH types of Vikings!
    private VikingMovement meleeScript;
    private RangedViking rangedScript;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        vikingCollider = GetComponent<Collider>();

        meleeScript = GetComponent<VikingMovement>();
        rangedScript = GetComponent<RangedViking>(); // Finds the new script!
    }

    public void TakeDamage(int damageAmount)
    {
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

        // --- The Village Manager now tracks death automatically! ---

        // Count this kill for the Game Over stats!
        GameOverManager.AddKill();

        // 1. Play the Animator "Die" Trigger we set up earlier
        animator.SetTrigger("Die");

        // 2. Turn off the NavMesh so he stops moving
        if (agent != null) agent.enabled = false;

        // 3. Turn off his brain so he stops swinging axes OR throwing spears!
        if (meleeScript != null) meleeScript.enabled = false;
        if (rangedScript != null) rangedScript.enabled = false;

        // 4. Turn off the Collider so you can walk over his body
        if (vikingCollider != null) vikingCollider.enabled = false;

        // 5. Sink into the ground/destroy after 5 seconds to clear memory
        Destroy(gameObject, 5f);
    }
}