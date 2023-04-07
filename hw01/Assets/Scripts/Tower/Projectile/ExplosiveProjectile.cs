using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    private float _range;
    private int _damage;
    private void Start()
    {
        _range = 5;
        _damage = 10;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null)
        {
            return;
        }
        
        enemy.Health.HealthValue -= _damage;
        var hits = Physics.SphereCastAll
            (transform.position, _range, Vector3.forward, 0, _enemyLayerMask);
        if (hits.Length == 0)
        {
            HandleDeath();
            return;
        }

        foreach (var hit in hits)
        {
            hit.transform.GetComponent<Enemy>().Health.HealthValue -= _damage;
        }
        HandleDeath();
    }
}
