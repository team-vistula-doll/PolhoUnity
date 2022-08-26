using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;

public class EnemyDanmakuCollider : DanmakuCollider
{
    public DanmakuCollider Collider;

    private IShootable _enemyEmitter;
    private IHitable _enemy;

    private void Start()
    {
        _enemyEmitter = GetComponentInChildren<IShootable>();
        _enemy = GetComponentInParent<IHitable>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (Collider == null) return;
        Debug.Log("Subscribed");
        Collider.OnDanmakuCollision += OnDanmakuCollision;
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (Collider != null)
            Collider.OnDanmakuCollision -= OnDanmakuCollision;
    }

    new void OnDanmakuCollision(DanmakuCollisionList collisions)
    {
        bool hit = false;
        foreach (var collision in collisions)
        {
            if (collision.Danmaku.Pool != _enemyEmitter.Set.Pool) //Ignore enemy emitter
            {
                hit = true;
                collision.Danmaku.Destroy();
            }
        }

        if (hit)
            _enemy.OnHit();
    }

    /// <summary>
    /// Reset is called when the user hits the Reset button in the Inspector's
    /// context menu or when adding the component the first time.
    /// </summary>
    void Reset()
    {
        Collider = GetComponent<DanmakuCollider>();
    }
}
