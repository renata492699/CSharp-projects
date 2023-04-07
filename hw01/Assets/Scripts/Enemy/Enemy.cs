using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementComponent), typeof(HealthComponent), typeof(BoxCollider))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected MovementComponent _movementComponent;
    [SerializeField] protected HealthComponent _healthComponent;
    [SerializeField] protected ParticleSystem _onDeathParticlePrefab;
    [SerializeField] protected ParticleSystem _onSuccessParticlePrefab;
    [SerializeField] protected LayerMask _attackLayerMask;
    [SerializeField] protected float _speed;
    [SerializeField] protected int _reward;

    public event Action OnDeath;
    
    protected RaycastHit Target;
    protected int CollisionTower;
    protected int CollisionCastle;

    public HealthComponent Health => _healthComponent;

    private float Speed => _speed;
    private int Reward => _reward;

    protected void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
        _movementComponent.MoveAlongPath();
    }

    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    public void Init(EnemyPath path)
    {
        _movementComponent.Init(path, Speed);
    }

    private void HandleDeath()
    {
        var particle = Instantiate(_onDeathParticlePrefab, transform.position, transform.rotation);
        particle.Play();
        
        FindObjectOfType<Player>().Resources += Reward;
        OnDeath?.Invoke();
        Destroy(gameObject);
        Destroy(particle);
    }
    
    protected void OnCollisionEnter(Collision other)
    {
        var tower = other.gameObject.GetComponent<Tower>();
        var castle = other.gameObject.GetComponent<Castle>();
        if (tower != null)
        {
            tower.Health.HealthValue -= CollisionTower;
        }

        if (castle != null)
        {
            castle.Health.HealthValue -= CollisionCastle;
        }
        HandleDeath();
    }
}
