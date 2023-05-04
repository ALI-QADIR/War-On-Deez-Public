using Assets.Scripts.HealthAndDamage;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealth : Health
    {
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            // TODO: Blaze AI hit
        }

        public override void Die()
        {
            // TODO: Blaze AI Die.
        }
    }
}