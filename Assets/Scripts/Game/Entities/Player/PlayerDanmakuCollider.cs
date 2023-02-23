using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DanmakU;
using UnityEngine;

public class PlayerDanmakuCollider : DanmakuCollider
{
    public DanmakuCollider Collider;
    public bool IsInvincible = false;

    private IShootable _playerEmitter;
    private PlayerDanmakuEmitter playerEmitter;
    private Player player;

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerEmitter = GetComponentInChildren<PlayerDanmakuEmitter>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsInvincible)
            StartCoroutine(player.OnHit());
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
        
        foreach (var collision in collisions.Where(collision => collision.Danmaku.Pool != playerEmitter.Set.Pool))
        {
            hit = true;
            collision.Danmaku.Destroy();
        }

        if (hit && !IsInvincible)
            StartCoroutine(player.OnHit());
    }
    
    void Reset()
    {
        Collider = GetComponent<DanmakuCollider>();
    }
}
