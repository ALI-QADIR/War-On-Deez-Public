using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Enemy;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    // Update is called once per frame

    //private void Start()
    //{
    //    enemyHealth = new EnemyHealth
    //    {
    //        maxHealth = 100,
    //        currentHealth = 100
    //    };
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            enemyHealth.TakeDamage(10);
        }
    }
}