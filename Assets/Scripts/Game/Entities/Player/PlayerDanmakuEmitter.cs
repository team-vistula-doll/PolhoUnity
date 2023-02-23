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

    private DanmakuConfig config;
    private IFireable fireable;
    private Player player;
        
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        if (DanmakuType == null)
        {
            Debug.LogWarning($"Emitter doesn't have a valid DanmakuPrefab", this);
            return;
        }
        Set = CreateSet(DanmakuType);
        Set.AddModifiers(GetComponents<IDanmakuModifier>());
        Fireable = Arc.Of(Line).Of(Set);
    }
    
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
            if (player.ShootSound)
                Player._AudioSource.PlayOneShot(player.ShootSound);
            Timer = 1f / FireRate.GetValue();
        }
    }

}
