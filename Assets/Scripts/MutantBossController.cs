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

    [Header("Epic Attack Effects")]
    public GameObject wildFirePrefab; // Drag the fire prefab here
    public int jumpAttackDamage = 40;
    public int fireDamagePerSecond = 10;

    private float lastPunchTime;
    private Animator animator;
    private Transform player;

    // Effect Flags
    private bool hasDoneSwipeEffect = false;
    private bool hasDoneJumpEffect = false;
    private bool hasDoneRoarEffect = false;


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

        // 1. ORIGINAL AI LOGIC
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
                animator.SetTrigger("Punch"); // Triggers the user's custom animator sequence!
                lastPunchTime = Time.time;
                
                // Start the swipe effect and damage timer immediately!
                StartCoroutine(DoSwipeEffect(distance));
            }
        }

        // 2. PASSIVE EFFECT WATCHER
        // We watch the animator state and inject our epic effects at the perfect time!
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Mutant Jumping (1)"))
        {
            if (!hasDoneJumpEffect) { StartCoroutine(DoJumpEffect(distance)); hasDoneJumpEffect = true; }
            hasDoneRoarEffect = false;
        }
        else if (stateInfo.IsName("Mutant Roaring"))
        {
            if (!hasDoneRoarEffect) { StartCoroutine(DoRoarEffect(distance)); hasDoneRoarEffect = true; }
            hasDoneSwipeEffect = false; hasDoneJumpEffect = false;
        }
        else
        {
            hasDoneJumpEffect = false; hasDoneRoarEffect = false;
        }
    }

    private System.Collections.IEnumerator DoSwipeEffect(float currentDistance)
    {
        // Wait for the exact moment the slice hits the player
        yield return new WaitForSeconds(damageDelay);
        DealPunchDamage();

        // EFFECT: The Wind-Tunnel Slice!
        if (Camera.main != null)
            Camera.main.transform.DOShakePosition(0.3f, new Vector3(0.5f, 0.5f, 0f), 15, 90f);
        
        if (DamageVignette.Instance != null)
            DamageVignette.Instance.Flash();
    }

    private System.Collections.IEnumerator DoJumpEffect(float currentDistance)
    {
        // Wait until the jump animation physically reaches the landing frame (roughly 60% complete)
        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            
            // If the animation changes or finishes early, break out
            if (!stateInfo.IsName("Mutant Jumping (1)")) break; 
            
            // 0.6f normalized time is usually the exact moment feet hit the ground in a heavy slam
            if (stateInfo.normalizedTime >= 0.6f) break; 
            
            yield return null; // Wait for the next frame
        }

        // EFFECT: The Earthquake! (Made much stronger!)
        if (Camera.main != null)
            Camera.main.transform.DOShakePosition(1.5f, new Vector3(0, 8f, 0), 30, 90f); // Insane vertical shake

        // Deal AOE damage to the player if they are nearby when he hits the ground
        if (Vector3.Distance(transform.position, player.position) <= punchDistance * 2f)
        {
            PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
            if (playerHP != null) playerHP.TakeDamage(jumpAttackDamage);
        }
    }

    private System.Collections.IEnumerator DoRoarEffect(float currentDistance)
    {
        // Wait 3.5 seconds into the roar animation before shooting fire!
        yield return new WaitForSeconds(3.5f);

        // EFFECT: Dragon's Breath!
        if (wildFirePrefab != null)
        {
            Transform headBone = null;
            if (animator.isHuman) headBone = animator.GetBoneTransform(HumanBodyBones.Head);
            
            if (headBone == null)
            {
                // Prioritize Jaw/Mouth
                foreach(Transform t in GetComponentsInChildren<Transform>())
                {
                    string n = t.name.ToLower();
                    if (n.Contains("jaw") || n.Contains("mouth")) { headBone = t; break; }
                }
                // Fallback to Head
                if (headBone == null)
                {
                    foreach(Transform t in GetComponentsInChildren<Transform>())
                    {
                        if (t.name.ToLower().Contains("head")) { headBone = t; break; }
                    }
                }
            }

            Vector3 basePos = headBone != null ? headBone.position : transform.position + Vector3.up * 15f;
            
            GameObject fire = Instantiate(wildFirePrefab, basePos, transform.rotation);
            
            if (headBone != null) 
            {
                fire.transform.SetParent(headBone);
                // Push the fire forward out of the face center so it's not trapped inside his body!
                fire.transform.position = headBone.position + (transform.forward * 8f) + (Vector3.down * 2f);
            }
            else 
            {
                fire.transform.SetParent(transform);
            }

            // Aim diagonally downwards to the floor
            fire.transform.rotation = Quaternion.LookRotation(transform.forward + (Vector3.down * 1.5f));

            // Toned down the fire to look better and not fill the whole screen!
            ParticleSystem[] pSystems = fire.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem ps in pSystems)
            {
                var main = ps.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy; 
                main.startSizeMultiplier *= 4f;      // Much more reasonable size
                main.startSpeedMultiplier *= 10f;    // Still fast, but not chaotic
                main.startLifetimeMultiplier *= 2f;  // Reaches the floor
                main.gravityModifierMultiplier = 1.5f; // Pulls down to the floor smoothly
            }

            fire.transform.localScale = Vector3.one * 5f; // Reduced base scale

            Destroy(fire, 5f);
        }

        // Deal continuous fire damage while roaring
        // Tick damage 5 times over the next 3 seconds while the fire is blasting
        for(int i = 0; i < 5; i++)
        {
            // The fire shoots very far, so we use a massive damage radius (punchDistance * 4)
            if (Vector3.Distance(transform.position, player.position) <= punchDistance * 4f)
            {
                PlayerHealth playerHP = player.GetComponent<PlayerHealth>();
                if (playerHP != null) playerHP.TakeDamage(fireDamagePerSecond);
            }
            yield return new WaitForSeconds(0.6f);
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