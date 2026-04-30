using UnityEngine;

public class SpearProjectile : MonoBehaviour
{
    public int damage = 20;

    void Start()
    {
        // Just destroy it after 5 seconds if it misses
        Destroy(gameObject, 5f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Untagged")) return;

        PlayerHealth playerHP = other.GetComponent<PlayerHealth>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}