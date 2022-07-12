using System;
using System.Collections.Generic;
using DanmakU;
using UnityEngine;

public class EnemyDanmakuSet : DanmakuBehaviour
{
    public DanmakuPrefab DanmakuType;
    public static DanmakuSet Set { get; private set; }

    public void Start()
    {
        if (DanmakuType == null)
        {
            Debug.LogWarning($"Emitter doesn't have a valid DanmakuPrefab", this);
            return;
        }
        Set = CreateSet(DanmakuType);
    }
}
