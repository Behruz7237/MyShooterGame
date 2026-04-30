using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interactions
{
    public class Swordinteraction : MonoBehaviour
    {
        [SerializeField] private GameObject _impactEffect;

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("SWORD TOUcHED " + other.gameObject.name);

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
        }
    }
}