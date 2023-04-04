using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathData : MonoBehaviour
    {
        public List<Vector2> Path = new() { Vector2.zero };
    }
}