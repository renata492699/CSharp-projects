using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BurstTower : Tower
{
    private bool _firstProjectile = true;

    private float _shootingTime;
    private float _projectileTime;

    private new void Start()
    {
        base.Start();
        Health.HealthValue = 120;
        Range = 10;
        TimeBetween = 3f;
        _projectileTime = 0.2f;
        _shootingTime = 3f;
    }

    private Transform ChooseEnemy(RaycastHit[] raycastHits)
    {
        var strongest = raycastHits[0].transform;
        foreach (var hit in raycastHits)
        {
            if (hit.transform == _objectToPan)
            {
                return _objectToPan;
            }
            var hitStrength = hit.transform.GetComponent<Enemy>().Health.HealthValue;
            if (hitStrength > strongest.transform.GetComponent<Enemy>().Health.HealthValue)
            {
                strongest = hit.transform;
            }
        }
        return strongest;
    }

    private void Update()
    {
        if (!ShootTime(_shootingTime))
        {
            return;
        }
        
        if (!HitTarget())
        {
            return;
        }
        
        _objectToPan = ChooseEnemy(Hits);

        _shootingTime = (_firstProjectile) ? _projectileTime : TimeBetween;
        _firstProjectile = !_firstProjectile;

        Shoot(_objectToPan);
    }
}
