using System;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float health = 10f;
    [SerializeField] float attack = 2f;
    EnemyControl enemyControl;
    private void Awake()
    {
        enemyControl = GetComponent<EnemyControl>();
    }

    internal void DealDamage(GameObject collision)
    {
        if (collision.CompareTag("Player") && enemyControl.isHit)
        {
            SoldierAttack soldier = collision.GetComponent<SoldierAttack>();
            soldier.ReceiveDamage(attack);
        }
    }
    internal void ReceiveDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
            Death();
    }

    private void Death()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        DealDamage(collision.gameObject);
    }
}
