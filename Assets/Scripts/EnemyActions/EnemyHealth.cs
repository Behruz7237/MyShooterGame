using UnityEngine;

namespace Assets.Scripts.EnemyActions
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private int health = 3;

        /// <summary>
        /// Reduces the enemy's health by the specified damage amount.
        /// Destroys the enemy GameObject when health drops to 0 or below.
        /// </summary>
        /// <param name="damage">Amount of damage to take.</param>
        public void TakeDamage(int damage)
        {
            health -= damage;
            
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
