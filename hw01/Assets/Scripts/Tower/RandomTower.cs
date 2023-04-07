using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class RandomTower : Tower
{
    private new void Start()
    {
        base.Start();
        Health.HealthValue = 140;
        Range = 10;
        TimeBetween = 2f;
    }

    private new void Shoot(Transform target)
    {
        Timer = 0f;
        var chance = Random.Range(0, 100);
        _projectileSpawn.LookAt(target.position);
        switch (chance)
        {
            case < 20:
            {
                var projectile1 = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation) ?? throw new ArgumentNullException("Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation)");
                var projectile2 = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation);
                projectile1.GetComponent<Rigidbody>().velocity = _projectileSpawn.forward * projectile1.Speed;
                projectile2.GetComponent<Rigidbody>().velocity = _projectileSpawn.forward * projectile2.Speed;
                break;
            }
            case < 60:
            {
                var projectile = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation);
                projectile.GetComponent<Rigidbody>().velocity = _projectileSpawn.forward * projectile.Speed;
                break;
            }
        }
    }

    private Transform ChooseEnemy(RaycastHit[] raycastHits)
    {
        foreach (var hit in raycastHits)
        {
            if (hit.transform == _objectToPan)
            {
                return _objectToPan;
            }
        }
        var index = Random.Range(0, raycastHits.Length);
        return raycastHits[index].transform;
    }

    private void Update()
    {
        if (!ShootTime(TimeBetween))
        {
            return;
        }
        
        if (!HitTarget())
        {
            return;
        }
        
        _objectToPan = ChooseEnemy(Hits);

        Shoot(_objectToPan);
    }
}
