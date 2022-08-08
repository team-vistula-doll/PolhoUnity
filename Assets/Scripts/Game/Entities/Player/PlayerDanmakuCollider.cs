using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;

public class PlayerDanmakuCollider : DanmakuCollider
{
    public DanmakuCollider Collider;
    public bool IsInvincible = false;
    public Player player;

    private IShootable _playerEmitter;
    private PlayerDanmakuEmitter playerEmitter;

    private void Start()
    {
        playerEmitter = GetComponentInChildren<PlayerDanmakuEmitter>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsInvincible)
            StartCoroutine(player.OnHit());
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
            if(collision.Danmaku.Pool != playerEmitter.Set.Pool) //Ignore player emitter
            {
                hit = true;
                collision.Danmaku.Destroy();
            }
            
        }

        if (hit && !IsInvincible)
            StartCoroutine(player.OnHit());
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
