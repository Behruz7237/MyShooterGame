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
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }

            else if (other.CompareTag("Player"))
            {
                //var healtController = GetComponent<HealthController>();
                Debug.Log("Player is being shot");
            }

                DestroyItself();
        }
    }
}