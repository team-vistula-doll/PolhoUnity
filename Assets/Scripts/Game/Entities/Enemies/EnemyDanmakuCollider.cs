using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanmakU;
using UnityEngine;

public class EnemyDanmakuCollider : DanmakuCollider
{
    public DanmakuCollider Collider;

    private IShootable _enemyEmitter;
    private IEntity _enemy;

    private void Start()
    {
        _enemyEmitter = GetComponentInChildren<IShootable>();
        _enemy = GetComponentInParent<IEntity>();
    }
    
    void OnEnable()
    {
        if (Collider != null)
            Collider.OnDanmakuCollision += OnDanmakuCollision;
    }
    
    void OnDisable()
    {
        if (Collider != null)
            Collider.OnDanmakuCollision -= OnDanmakuCollision;
    }

    new void OnDanmakuCollision(DanmakuCollisionList collisions)
    {
        bool hit = false;

        foreach (var col in collisions.Where(c => c.Danmaku.Pool != _enemyEmitter.Set.Pool))
        {
            hit = true;
            col.Danmaku.Destroy();
        }

        if (hit)
            _enemy.OnHit();
    }
    
    void Reset()
    {
        Collider = GetComponent<DanmakuCollider>();
    }
}
