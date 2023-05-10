using System.Collections.Generic;
using UnityEngine;
using Bezier;
using System;

namespace WaypointPath
{
    /// <summary>
    /// A structure for constructing at most cubic Bezier curves
    /// </summary>
    public struct BezierControlPoints
    {
        public Vector2 StartPosition;
        public Vector2 EndPosition;
        public Vector2 StartControl;
        public Vector2 EndControl;

        public BezierControlPoints(Vector2 startPosition, Vector2 endPosition, Vector2 startControl, Vector2 endControl)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartControl = startControl;
            EndControl = endControl;
        }

        /// <summary>
        /// Copies and modifies the curve; for simple operations that change all points
        /// </summary>
        /// <param name="vector">What vector to use</param>
        /// <param name="mod">The operation to do, e.g. <c>(x, y) => x + y</c>, where 'x' is the curve control points 
        /// and 'y' is <paramref name="vector"/></param>
        /// <returns>A new modified Bezier curve</returns>
        public BezierControlPoints GetModifiedCurveCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPosition, vector);
             Vector2 endPosition = mod(EndPosition, vector);
             Vector2 startControl = mod(StartControl, vector);
             Vector2 endControl = mod(EndControl, vector);
            return new BezierControlPoints(startPosition, endPosition, startControl, endControl);
        }

        /// <summary>
        /// Modifies the curve; for simple operations that change all points
        /// </summary>
        /// <param name="vector">What vector to use</param>
        /// <param name="mod">The operation to do, e.g. <c>(x, y) => x + y</c>, where 'x' is the curve control points 
        /// and 'y' is <paramref name="vector"/></param>
        public void ModifyCurve(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            StartPosition = mod(StartPosition, vector);
            EndPosition = mod(EndPosition, vector);
            StartControl = mod(StartControl, vector);
            EndControl = mod(EndControl, vector);
        }
    }

    public abstract class WaypointPathCreator
    //Given starting position to a selected function,
    //generates List of waypoints (Vectors2) that WaypointWalkers will follow
    {
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

        public List<Vector2> GeneratePathFromCurve(BezierControlPoints bezierControlPoints, float stepSize = 0.5f)
        {
            if (bezierControlPoints.EndPosition == Vector2.zero)
                return new List<Vector2>() { Vector2.zero };

            if (stepSize <= 0.1f) stepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            List<Vector2> waypoints = new();
            if (bezierControlPoints.EndControl != Vector2.zero)
            {
                if (bezierControlPoints.StartControl != Vector2.zero)
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.CubicCurve(bezierControlPoints.StartPosition, bezierControlPoints.StartControl,
                            bezierControlPoints.StartControl, bezierControlPoints.EndPosition, t * stepSize / 100));
                    }
                }
                else
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.QuadraticCurve(bezierControlPoints.StartPosition, bezierControlPoints.StartControl,
                            bezierControlPoints.EndPosition, t * stepSize / 100));
                    }
                }
            }
            else
            {
                waypoints.Add(BezierCurve.Lerp(bezierControlPoints.StartPosition, bezierControlPoints.EndPosition, 1));
            }

            return waypoints;
        }

        public abstract List<Vector2> GeneratePath(Vector2 startPos, float stepSize = 0.5f);
    }
}