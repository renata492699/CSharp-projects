using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected Rigidbody _rb;
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] protected ParticleSystem _onHitParticleSystem;
    [SerializeField] protected float _speed;
    [SerializeField] protected float _life;
    
    public float Speed => _speed;
    private float Life => _life;
    protected Enemy enemy;


    private void Awake()
    {
        Destroy(gameObject, Life);
    }

    protected void HandleDeath()
    {
        
        var particle = Instantiate(_onHitParticleSystem, enemy.transform.position, enemy.transform.rotation);
        particle.Play();
        
        Destroy(gameObject);
        Destroy(particle);

    }
}
