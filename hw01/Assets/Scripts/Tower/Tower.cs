using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(BoxCollider), typeof(HealthComponent))]
public class Tower : MonoBehaviour
{
    [SerializeField] protected LayerMask _enemyLayerMask;
    [SerializeField] private HealthComponent _healthComponent;
    [SerializeField] protected Projectile _projectilePrefab;
    [SerializeField] private BoxCollider _boxCollider;
    [SerializeField] protected Transform _objectToPan;
    [SerializeField] protected Transform _projectileSpawn;
    [SerializeField] private GameObject _previewPrefab;
    [SerializeField] protected int _price;

    public HealthComponent Health => _healthComponent;
    public BoxCollider Collider => _boxCollider;
    public GameObject BuildingPreview => _previewPrefab;

    protected int Range;
    protected float TimeBetween;
    protected float Timer;
    protected RaycastHit[] Hits;

    public int Price => _price;

    protected void Start()
    {
        _healthComponent.OnDeath += HandleDeath;
    }

    protected bool ShootTime(float time)
    {
        Timer += Time.deltaTime;
        return !(Timer < time);
    }
    
    protected bool HitTarget()
    {
        Hits = Physics.SphereCastAll
            (transform.position, Range, Vector3.forward, 0, _enemyLayerMask);
        return Hits.Length != 0;
    }
    
    protected void Shoot(Transform target)
    {
        Timer = 0f;
        _projectileSpawn.LookAt(target.position);
        var projectile = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation);
        projectile.GetComponent<Rigidbody>().velocity = _projectileSpawn.forward * projectile.Speed;
    }
    
    private void OnDestroy()
    {
        _healthComponent.OnDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        Destroy(gameObject);
    }
}
