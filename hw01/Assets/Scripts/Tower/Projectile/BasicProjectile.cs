using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicProjectile : Projectile
{
    private int _damage;
    private void Start()
    {
        _damage = 25;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }
        
        enemy.Health.HealthValue -= _damage;
        HandleDeath();
    }
}
