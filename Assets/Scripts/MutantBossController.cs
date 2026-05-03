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
    private bool isAttacking = false;


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

        // Only rotate and start an attack if we are close enough and NOT already attacking
        if (distance <= punchDistance && !isAttacking)
        {
            Vector3 lookDirection = (player.position - transform.position).normalized;
            lookDirection.y = 0;
            if (lookDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * 5f);
            }

            if (Time.time >= lastPunchTime + punchCooldown)
            {
                StartCoroutine(AttackRoutine(lookDirection, distance));
            }
        }
    }

    private System.Collections.IEnumerator AttackRoutine(Vector3 lookDirection, float currentDistance)
    {
        isAttacking = true;
        animator.SetTrigger("Punch");
        lastPunchTime = Time.time;

        // Wait for the exact moment the slice hits the player
        yield return new WaitForSeconds(damageDelay);
        DealPunchDamage();

        // To ensure the boss does the "slicing animation till the end before he starts the next one",
        // we enforce a strict waiting period. Giant animations played slowly (0.4x speed) take a long time!
        float finishWaitTime = punchCooldown - damageDelay;
        if (finishWaitTime < 3.5f) finishWaitTime = 3.5f; // Force a minimum 3.5s finish time to prevent clipping

        yield return new WaitForSeconds(finishWaitTime);

        isAttacking = false;
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

        // Delay Victory UI!
        Invoke("ShowVictoryUI", 7f);

        Destroy(gameObject, 10f);
    }

    private void ShowVictoryUI()
    {
        GameOverManager manager = Object.FindFirstObjectByType<GameOverManager>();
        if (manager != null) manager.TriggerVictory("RAGNAROK");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, punchDistance);
    }
}