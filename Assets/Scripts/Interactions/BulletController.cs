using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class BulletController : MonoBehaviour
    {
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject _impactEffect;

        private float _moveSpeed = 20f; 
        
        public void Init(float moveSpeed)
        {
            _moveSpeed = moveSpeed;
            StartCoroutine(CountCoroutine());
        }

        private IEnumerator CountCoroutine()
        {
            yield return new WaitForSeconds(lifeTime);
            DestroyItself(false);
        }

        private void Update()
        {
            rb.linearVelocity = transform.forward * _moveSpeed;
        }

        private void DestroyItself(bool shouldPlayEfect = true)
        {
            Destroy(gameObject);
            if (!shouldPlayEfect) return;
            Instantiate(_impactEffect, transform.position + (transform.forward * (-_moveSpeed * Time.deltaTime)), transform.rotation);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 1. IGNORE INVISIBLE VOLUMES (Lighting, fog, wind zones, etc.)
            if (other.isTrigger) return;

            // 2. IGNORE THE PLAYER AND GUN
            if (other.CompareTag("Player") || other.transform.root.CompareTag("Player")) return;

            // 3. IF ENEMY, DESTROY IT
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }

            // Inside your BulletController's OnTriggerEnter or OnCollisionEnter function:

            VikingHealth viking = other.GetComponent<VikingHealth>();
            if (viking != null)
            {
                viking.TakeDamage(35); // Or whatever damage you want!
            }

            // 4. Disable our own collider so it physically CANNOT explode 4 times in one frame
            GetComponent<Collider>().enabled = false;

            DestroyItself();
        }
    }
}