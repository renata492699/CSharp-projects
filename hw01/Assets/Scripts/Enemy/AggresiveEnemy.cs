using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AggresiveEnemy : Enemy
{
    private new void Start()
    {
        base.Start();
        Health.HealthValue = 100;
        CollisionCastle = 40;
        CollisionTower = 40;
    }

    private void Update()
    {
        var hits = Physics.SphereCastAll
            (transform.position, 10, Vector3.forward, 0, _attackLayerMask);
        if (hits.Length == 0)
        {
            _movementComponent.MoveAlongPath();
            return;
        }

        if (!hits.Contains(Target))
        {
            Target = hits[0];    
        }
        _movementComponent.MoveTowards(Target.transform);
    }
}
