using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LazyEnemy : Enemy
{
    private const float Wait = 2f;
    private const float Move = 5f;
    private float _timer;

    private new void Start()
    {
        base.Start();
        Health.HealthValue = 175;
        CollisionTower = 50;
        CollisionCastle = 25;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_movementComponent.IsMoving)
        {
            if (_timer >= Move)
            {
                _movementComponent.CancelMovement();
                _timer = 0f;
                return;
            }
            _movementComponent.MoveAlongPath();
        }

        if (_movementComponent.IsMoving || !(_timer >= Wait)) return;
        _timer = 0f;
        _movementComponent.MoveAlongPath();
    }
}
