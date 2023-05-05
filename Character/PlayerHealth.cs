using UnityEngine;
using Assets.Scripts.HealthAndDamage;

namespace Assets.Scripts.Character
{
    public class PlayerHealth : Health
    {
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            // TODO: add hit effect in UI
        }

        public override void Die()
        {
            // TODO: play Death animation call end game function
            Debug.Log("Player Died");
        }

        public void Heal(float healAmount)
        {
            if (currentHealth + healAmount > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += healAmount;
            }
        }
    }
}