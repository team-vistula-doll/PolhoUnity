using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EnemyStruct
{
    public struct Enemy
    {
        public int ID;
        public string Name;
        public float SpawnTime;
        public Vector2 SpawnPosition;
        public List<List<Vector2>> Waypoints;
        public List<(float delay, int amount)> SpawnRepeats; //delay between spawns, amount of spawns; Optional
    }
}
