using UnityEngine;

namespace Assets.Scripts.EnemyActions
{
    /// <summary>
    /// Attach to the EnemyBullet prefab (a small Sphere).
    /// Requires: Rigidbody (UseGravity=false), SphereCollider (IsTrigger=true).
    /// Flies forward, damages the player on contact, and destroys itself.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class EnemyBullet : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private float speed = 15f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float lifeTime = 5f;

        private bool hasHit = false;

        private void Start()
        {
            // Ensure the bullet is active (for runtime-created prefabs)
            gameObject.SetActive(true);

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.linearVelocity = transform.forward * speed;
            }
            
            // Auto-destroy after lifetime expires
            Destroy(gameObject, lifeTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // Prevent multiple hits
            if (hasHit) return;

            // Don't hit other enemies
            if (other.CompareTag("Enemy")) return;

            // Damage the player if we hit them
            PlayerHealth playerHP = other.GetComponent<PlayerHealth>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
                Debug.Log("Enemy bullet hit the player for " + damage + " damage!");
            }

            hasHit = true;
            Destroy(gameObject);
        }
    }
}
