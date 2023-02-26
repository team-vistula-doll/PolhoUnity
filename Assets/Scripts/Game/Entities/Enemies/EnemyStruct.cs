using System;
using System.Collections.Generic;
using UnityEngine;
using DanmakU;

namespace EnemyStruct
{
    [Serializable]
    public struct Enemy
    {
        public int ID;
        public string Name;
        public float SpawnTime;
        public Vector2 SpawnPosition;
        [HideInInspector]
        public List<Vector2> Path;
        public List<(float delay, int amount)> SpawnRepeats; //delay between spawns, amount of spawns; Optional
        public IFireable Fireable;
    }
}
