using System.Collections.Generic;
using UnityEngine;
using Bezier;
using System;

namespace WaypointPath
{
    public abstract class PathProperties
    {
        [Delayed]
        public Vector2 StartPosition;

        public abstract PathProperties GetNewAdjoinedPath(float percent);
    }

    public abstract class WaypointPathCreator
    //Given starting position to a selected function,
    //generates List of waypoints (Path2) that WaypointWalkers will follow
    {
        public PathProperties Properties;
        /// <summary>
        /// Creates a Vector2 from its length and angle
        /// </summary>
        /// <param name="length">Magnitude of the vector; if <= 0 defaults to 1</param>
        /// <param name="angle">Direction of the vector in degrees</param>
        public Vector2 CreateVector2(float length, float angle)
        {
            if (length <= 0) length = 1;
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length;
        }

        public abstract List<Vector2> GeneratePath(PathProperties properties, float stepSize = 0.5f);
        public abstract List<Vector2> GeneratePath(float stepSize = 0.5f);
    }
}