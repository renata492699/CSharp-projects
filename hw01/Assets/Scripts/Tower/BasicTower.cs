using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

public class BasicTower : Tower
{
    private new void Start()
    {
        base.Start();
        Health.HealthValue = 50;
        Range = 20;
        TimeBetween = 1.5f;

    }

    private Transform ChooseEnemy(RaycastHit[] raycastHits)
    {
        var closest = raycastHits[0].transform;
        var distance = Vector3.Distance(closest.position, transform.position);
        foreach (var hit in raycastHits)
        {
            if (_objectToPan == hit.transform)
            {
                return _objectToPan;
            }
            var hitDistance = Vector3.Distance(hit.transform.position, transform.position);
            if (!(hitDistance < distance)) continue;
            closest = hit.transform;
            distance = hitDistance;
        }
        return closest;
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
