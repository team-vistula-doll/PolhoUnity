using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Fireables;
using UnityEngine;

public class EnemyDanmakuEmitter : DanmakuBehaviour, IShootable
{
    public DanmakuPrefab DanmakuType;
    public Range Speed = 5f;
    public Range AngularSpeed;
    public Color Color = Color.white;
    public Range FireRate;
    public Arc Arc;
    public Line Line;

    public DanmakuSet Set { get; set; }
    public float Timer { get; set; }

    float timer;
    DanmakuConfig config;
    IFireable fireable;
    
    void Start()
    {
        if (DanmakuType == null)
        {
            Debug.LogWarning($"Emitter doesn't have a valid DanmakuPrefab", this);
            return;
        }
        Set = EnemyDanmakuSet.Set;
        Set.AddModifiers(GetComponents<IDanmakuModifier>());
        fireable = Arc.Of(Line).Of(Set);
    }
    
    void Update()
    {
        if (fireable == null) return;
        Timer -= Time.deltaTime;
        if (Timer < 0)
        {
            config = new DanmakuConfig
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad,
                Speed = Speed,
                AngularSpeed = AngularSpeed,
                Color = Color
            };
            fireable.Fire(config);
            Timer = 1f / FireRate.GetValue();
        }
    }
}

