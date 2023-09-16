using System;
using System.Collections.Generic;
using UnityEngine;
using WaypointPath;
using DanmakU;

namespace EnemyClass
{
    [Serializable]
    public class Enemy
    {
        public int ID;
        public string Name = "enemy";
        public float SpawnTime = 0f;
        public Vector2 SpawnPosition = Vector2.zero;
        [HideInInspector]
        public List<WaypointPathCreator> Path = new();
        public List<(float delay, int amount)> SpawnRepeats; //delay between spawns, amount of spawns; Optional
        public IFireable Fireable;
    }
}
