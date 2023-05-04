using UnityEngine;

namespace Assets.Scripts.HealthAndDamage
{
    /// <summary>
    /// Has 2 public virtual methods, TakeDamage(int damage) and Die()
    /// Has 2 public fields, int maxHealth, current Health;
    /// </summary>
    public class Health : MonoBehaviour, IIDamageable
    {
        public int maxHealth;
        public float currentHealth;

        /// <summary>
        /// Takes an integer <paramref name="damage"/> as a parameter.
        /// Deducts the the <paramref name="damage"/> from the maxHealth.
        /// </summary>
        /// <param name="damage">damage to be dealt</param>
        /// <returns>Nothing</returns>
        public virtual void TakeDamage(int damage)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public virtual void Die()
        {
            // Handle Death behaviour
        }

        private void Start()
        {
            currentHealth = maxHealth;
        }
    }
}