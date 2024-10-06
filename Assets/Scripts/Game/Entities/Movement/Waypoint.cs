using UnityEngine;

namespace WaypointPath
{
    public struct Waypoint
    {
        public Vector2 Position;
        public float Speed;
        public float Acceleration;

        public Waypoint(Vector2 position, float speed, float acceleration)
        {
            Position = position;
            Speed = speed;
            Acceleration = acceleration;
        }
    }
}