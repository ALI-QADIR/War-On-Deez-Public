using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyHealth enemyHealth;

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerSword")
        {
            Debug.Log("Player hit");
            enemyHealth.TakeDamage(10);
        }
    }
}