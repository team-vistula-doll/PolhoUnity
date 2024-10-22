using UnityEngine;

namespace WaypointPath
{
    public struct Waypoint
    {
        public Vector2 Position;
        public float? Speed;
        public float? Acceleration;

        public Waypoint(Vector2 position, float? speed, float? acceleration)
        {
            Position = position;
            Speed = speed;
            Acceleration = acceleration;
        }
    }

    public static class WaypointMovement
    {
        /// <summary>
        /// Call every frame to calculate the acceleration needed to achieve an end speed
        /// <para/> Formula: <c>a = (V0^2 - V1^2) / (2 * d - V0 * deltatime)</c>
        /// </summary>
        /// <param name="currentSpeed">V0</param>
        /// <param name="endSpeed">V1</param>
        /// <param name="distanceLeft">d</param>
        /// <returns>a</returns>
        public static float AccelerateToSpeed(float currentSpeed, float endSpeed, float distanceLeft)
        {
            return (currentSpeed * currentSpeed - endSpeed * endSpeed) / (2 * distanceLeft - currentSpeed * Time.deltaTime);
        }
    }
}