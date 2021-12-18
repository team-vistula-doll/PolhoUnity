using System.Collections;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;

[RequireComponent(typeof(IShootable))]
public class PlayerDanmakuCollider : DanmakuCollider
{
    public DanmakuCollider Collider;
    private IShootable _playerEmitter;

    private void Start()
    {
        _playerEmitter = GetComponentInChildren<PDanmakuEmitter>();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        if (Collider != null)
        {
            Debug.Log("Subscribed");
            Collider.OnDanmakuCollision += OnDanmakuCollision;
        }
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        if (Collider != null)
        {
            Collider.OnDanmakuCollision -= OnDanmakuCollision;
        }
    }

    new void OnDanmakuCollision(DanmakuCollisionList collisions)
    {
        bool hit = false;
        foreach (var collision in collisions)
        {
            if(collision.Danmaku.Pool != _playerEmitter.Set.Pool) //Ignore player emitter
            {
                hit = true;
                collision.Danmaku.Destroy();
            }
            
        }

        if (hit)
        {
            GameStateManager.Instance.Lifes = GameStateManager.Instance.Lifes - 1;
            Debug.Log("Player hit!");
        }
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
