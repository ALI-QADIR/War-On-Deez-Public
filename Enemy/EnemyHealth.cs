using Assets.Scripts.HealthAndDamage;

namespace Assets.Scripts.Enemy
{
    public class EnemyHealth : Health
    {
        public override void Die()
        {
            BlazeAI blazeAi = GetComponent<BlazeAI>();
            if (blazeAi != null)
            {
                blazeAi.Death();
            }
        }
    }
}