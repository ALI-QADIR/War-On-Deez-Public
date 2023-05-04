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
        }

        /// <summary>
        /// Heals the player <paramref name="healAmount"/> amount of health.
        /// Divide 1 by heal rate and pass it to the method.
        /// call this method in a while loop => while(Heal(1/rate))
        /// </summary>
        /// <param name="healAmount"></param>
        /// <returns>Bool, true if current health is less than max health, false otherwise.</returns>
        public bool Heal(float healAmount)
        {
            currentHealth += healAmount;
            if (currentHealth < maxHealth) return true;
            currentHealth = maxHealth;
            return false;
        }
    }
}