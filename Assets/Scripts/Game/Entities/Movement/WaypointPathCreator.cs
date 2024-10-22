using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    [Serializable]
    public abstract class WaypointPathCreator
    //Given starting position and an implemented path creator,
    //will generate List of waypoints
    {
        public Waypoint StartPoint = new(Vector2.zero, 0, 0);
        [Range(0.5f, 50f)]
        public float StepSize = 0.5f;
        public List<(float percent, float speed, float acceleration)> KeyWaypoints = new();

        /// <summary>
        /// Copies and modifies the path; for simple operations that change all points
        /// </summary>
        /// <param name="vector">What vector to use</param>
        /// <param name="mod">The operation to do, e.g. <c>(x, y) => x + y</c>, where 'x' is the path control points 
        /// and 'y' is <paramref name="vector"/></param>
        public abstract WaypointPathCreator GetModifiedPathCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod);

        /// <summary>
        /// Modifies the path; for simple operations that change all points
        /// </summary>
        /// <param name="vector">What vector to use</param>
        /// <param name="mod">The operation to do, e.g. <c>(x, y) => x + y</c>, where 'x' is the path control points 
        /// and 'y' is <paramref name="vector"/></param>
        public abstract void ModifyPath(Vector2 vector, Func<Vector2, Vector2, Vector2> mod);

        public abstract WaypointPathCreator GetNewAdjoinedPath(float percent);

        public abstract Vector2? GetVectorAt(float percent);

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

        public abstract List<Waypoint> GeneratePath();
    }
}