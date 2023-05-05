using Assets.Scripts.Character;
using UnityEngine;

namespace Assets.Scripts.Enemy
{
    public class EnemyHitManager : MonoBehaviour
    {
        public int hitDamage = 5;
        public float hitDuration = 1.0f;
        private float _hitCountdown = 0.0f;

        public void Hit()
        {
            _hitCountdown = hitDuration;
        }

        private void Update()
        {
            if (_hitCountdown > 0.0f)
            {
                _hitCountdown -= Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && _hitCountdown > 0.0f)
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(hitDamage);
            }
        }
    }
}