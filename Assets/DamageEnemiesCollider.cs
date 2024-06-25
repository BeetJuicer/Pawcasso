using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemiesCollider : MonoBehaviour
{
    private float damage;

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);
        if(collision.gameObject.TryGetComponent<DemoEnemyControls>(out DemoEnemyControls enemy))
        {
            enemy.TakeDamage(damage);
        }
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
}
