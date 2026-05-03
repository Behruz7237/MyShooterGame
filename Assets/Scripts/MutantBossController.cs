using UnityEngine;
using UnityEngine.UI; // Needed for the Health Bar
using DG.Tweening;    // Needed for the smooth shrinking animation!

public class MutantBossController : MonoBehaviour
{
    [Header("Boss Health")]
    public int maxHealth = 500;
    private int currentHealth;
    private bool isDead = false;

    [Header("Boss UI")]
    public GameObject bossHealthUI;     // The Canvas that holds the health bar
    public Image healthFillImage;       // The Red Fill Image

    [Header("Animation Settings")]
    [Range(0.1f, 1f)]
    public float animationSpeed = 0.6f;

    [Header("Combat Settings")]
    public float punchDistance = 4f;
    public float punchCooldown = 3f;
    public int punchDamage = 25;
    public float damageDelay = 1.2f;

    private float lastPunchTime;
    private Animator animator;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        if (animator != null) animator.speed = animationSpeed;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        // When the Boss wakes up, turn on his epic health bar!
        if (bossHealthUI != null) bossHealthUI.SetActive(true);
        if (healthFillImage != null) healthFillImage.fillAmount = 1f;
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= punchDistance)
        {
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            if (Time.time >= lastPunchTime + punchCooldown)
            {
                animator.SetTrigger("Punch");
                lastPunchTime = Time.time;
                Invoke("DealPunchDamage", damageDelay);
            }
        }
    }

    private void DealPunchDamage()
    {
        if (isDead || player == null) return;

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

    public void TakeDamage(int damageAmount)
    {
        if (isDead) return;

        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;

        // Smoothly shrink the red health bar over 0.3 seconds!
        if (healthFillImage != null)
        {
            healthFillImage.DOFillAmount((float)currentHealth / maxHealth, 0.3f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        animator.ResetTrigger("Punch");
        animator.SetTrigger("Die");

        // Turn off the health bar when he dies!
        if (bossHealthUI != null) bossHealthUI.SetActive(false);

        Collider bossCollider = GetComponent<Collider>();
        if (bossCollider != null) bossCollider.enabled = false;

        Destroy(gameObject, 10f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchDistance);
    }
}