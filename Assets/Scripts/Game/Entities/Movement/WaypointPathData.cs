using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathData : MonoBehaviour
    {
        [SerializeReference]
        public List<WaypointPathCreator> Path = new();
    }
}