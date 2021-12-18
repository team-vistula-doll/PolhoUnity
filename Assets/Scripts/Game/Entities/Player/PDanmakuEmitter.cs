using System.Collections;
using System.Collections.Generic;
using DanmakU.Fireables;
using UnityEngine;

namespace DanmakU
{

    [AddComponentMenu("DanmakU/PDanmaku Emitter")]
    public class PDanmakuEmitter : DanmakuBehaviour, IShootable
    {

        public DanmakuPrefab DanmakuType;

        public Range Speed = 5f;
        public Range AngularSpeed;
        public Color Color = Color.white;
        [field: SerializeField]
        public Range FireRate { get; set; } = 5;
        public float FrameRate;
        public Arc Arc;
        public Line Line;

        public DanmakuSet Set { get; set; }

        public float Timer { get; set; }
        public bool CanShoot { get; set; } = false;

        DanmakuConfig config;
        IFireable fireable;

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
            fireable = Arc.Of(Line).Of(Set);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            if (fireable == null) return;
            var deltaTime = Time.deltaTime;
            if (FrameRate > 0)
            {
                deltaTime = 1f / FrameRate;
            }
            Timer -= deltaTime;
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
                fireable.Fire(config);
                Timer = 1f / FireRate.GetValue();
            }
        }

    }

}