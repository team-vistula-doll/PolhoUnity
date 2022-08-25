using System.Collections;
using System.Collections.Generic;
using DanmakU;
using DanmakU.Fireables;
using UnityEngine;

public class PlayerDanmakuEmitter : DanmakuBehaviour, IShootable
{

    public DanmakuPrefab DanmakuType;

    public Range Speed = 15f;
    public Range AngularSpeed;
    public Color Color = Color.white;
    public Arc Arc;
    public Line Line;

    [field: SerializeField]
    public Range FireRate { get; set; } = 5;
    public float Timer { get; set; }
    public DanmakuSet Set { get; set; }
    public bool CanShoot { get; set; } = false;    
    public IFireable Fireable { get; set; }
    public DanmakuConfig Config
    {
        get => config;
        set => config = value;
    }

    DanmakuConfig config;

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (DanmakuType == null)
        {
            Debug.LogWarning($"Emitter doesn't have a valid DanmakuPrefab", this);
            return;
        }
        Set = CreateSet(DanmakuType);
        Set.AddModifiers(GetComponents<IDanmakuModifier>());
        Fireable = Arc.Of(Line).Of(Set);
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if (Fireable == null) return;
        Timer -= Time.deltaTime;
        if (Timer < 0 && CanShoot)
        {
            config = new DanmakuConfig
            {
                Position = transform.position,
                Rotation = transform.rotation.eulerAngles.z * Mathf.Deg2Rad,
                Speed = Speed,
                AngularSpeed = AngularSpeed,
                Color = Color
            };
            Fireable.Fire(config);
            if (Player.shootSound)
                Player._AudioSource.PlayOneShot(Player.shootSound);
            Timer = 1f / FireRate.GetValue();
        }
    }

}