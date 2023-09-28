using System;
using System.Collections.Generic;
using UnityEngine;
using WaypointPath;
using DanmakU;

namespace EnemyClass
{
    [Serializable]
    public class Enemy : IComparable
    {
        public int ID;
        public string PrefabName = "Enemy";
        public string Name = "Enemy";
        [Min(0)]
        public float SpawnTime = 0f;
        public Vector2 SpawnPosition = Vector2.zero;
        [SerializeReference, NonReorderable]
        public List<WaypointPathCreator> Path = new();
        [SerializeReference]
        public List<(float delay, int amount)> SpawnRepeats; //delay between spawns, amount of spawns; Optional
        public IFireable Fireable;

        public int CompareTo(object obj)
        {
            Enemy enemy = obj as Enemy;
            return SpawnTime.CompareTo(enemy.SpawnTime);
        }
    }
}
