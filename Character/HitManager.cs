using Assets.Scripts.Enemy;
using UnityEngine;

namespace Assets.Scripts.Character
{
    public class HitManager : MonoBehaviour
    {
        public bool isAttacking;
        [HideInInspector] public int damage = 15;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Enemy") && isAttacking)
            {
                Debug.Log("Player Hit: " + other.gameObject.name);
                other.gameObject.GetComponent<EnemyHealth>().TakeDamage(damage);
                BlazeAI blazeAi = other.gameObject.GetComponent<BlazeAI>();
                if (blazeAi != null)
                {
                    blazeAi.Hit();
                }
            }
        }
    }
}