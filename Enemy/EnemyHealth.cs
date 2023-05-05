using Assets.Scripts.HealthAndDamage;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealth : Health
    {
        public override void Die()
        {
            Debug.Log("Enemy died");
            BlazeAI blazeAi = GetComponent<BlazeAI>();
            if (blazeAi != null)
            {
                blazeAi.Death();
            }
        }
    }
}