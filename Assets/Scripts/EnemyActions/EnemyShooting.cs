using UnityEngine;

namespace Assets.Scripts.EnemyActions
{
    /// <summary>
    /// Handles ranged shooting for enemies using visible projectiles.
    /// The player reference and FirePoint are found automatically so
    /// Prefab instances require ZERO manual setup.
    /// </summary>
    public class EnemyShooting : MonoBehaviour
    {
        [Header("Shooting Settings")]
        [Tooltip("Time in seconds between shots")]
        [SerializeField] private float fireRate = 2f;
        [Tooltip("How close the player needs to be before the enemy starts shooting")]
        [SerializeField] private float attackRange = 15f;
        
        [Header("Projectile Settings")]
        [Tooltip("The EnemyBullet prefab to spawn (auto-created if left empty)")]
        [SerializeField] private GameObject bulletPrefab;

        // Internal references — found automatically
        private Transform player;
        private Transform firePoint;

        private float nextFireTime;

        private void Start()
        {
            // ── Auto-find Player by tag ──
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning(gameObject.name + ": No GameObject with tag 'Player' found!");
            }

            // ── Auto-find or create FirePoint ──
            Transform existingFirePoint = transform.Find("FirePoint");
            if (existingFirePoint != null)
            {
                firePoint = existingFirePoint;
            }
            else
            {
                // Create FirePoint automatically at the front of the enemy
                GameObject fpObj = new GameObject("FirePoint");
                fpObj.transform.SetParent(transform);
                fpObj.transform.localPosition = new Vector3(0f, 0.8f, 0.6f); // Chest height, slightly forward
                firePoint = fpObj.transform;
            }

            // ── Auto-create bullet prefab if none assigned ──
            if (bulletPrefab == null)
            {
                bulletPrefab = CreateDefaultBulletPrefab();
            }
        }

        private void Update()
        {
            if (player == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                // Enemy physically rotates to face the player
                Vector3 lookDirection = player.position - transform.position;
                lookDirection.y = 0; // Keep rotation strictly horizontal
                if (lookDirection.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(lookDirection);
                }

                // Aim the fire point at the player's center mass
                Vector3 targetPos = player.position + Vector3.up * 1f;
                firePoint.LookAt(targetPos);

                // Fire on cooldown
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
            }
        }

        private void Shoot()
        {
            if (firePoint == null || bulletPrefab == null) return;

            // Spawn bullet at the fire point, aimed at the player
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }

        /// <summary>
        /// Creates a simple red sphere bullet at runtime if no prefab is assigned.
        /// This ensures enemies can ALWAYS shoot without manual setup.
        /// </summary>
        private GameObject CreateDefaultBulletPrefab()
        {
            GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bullet.name = "EnemyBullet_Runtime";
            bullet.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            // Make it red so the player can clearly see incoming fire
            Renderer rend = bullet.GetComponent<Renderer>();
            if (rend != null)
            {
                // Using Unlit/Color ensures the bullet works and is visible across
                // Standard, URP, and HDRP without shader errors.
                Shader shader = Shader.Find("Unlit/Color");
                if (shader == null) shader = Shader.Find("Standard");

                Material mat = new Material(shader);
                mat.color = Color.red;
                rend.material = mat;
            }

            // Set collider to trigger mode
            SphereCollider col = bullet.GetComponent<SphereCollider>();
            if (col != null) col.isTrigger = true;

            // Add Rigidbody
            Rigidbody rb = bullet.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;

            // Add the EnemyBullet script
            bullet.AddComponent<EnemyBullet>();

            // Deactivate it — Instantiate will clone it as active
            bullet.SetActive(false);

            return bullet;
        }
    }
}
