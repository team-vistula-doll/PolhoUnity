using Bezier;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    /// <summary>
    /// A class for constructing at most cubic Bezier curves
    /// </summary>
    public class BezierProperties : PathProperties
    {
        public Vector2 EndPosition;
        public Vector2 StartControl;
        public Vector2 EndControl;

        public BezierProperties(Vector2 startPosition, Vector2 endPosition, Vector2 startControl, Vector2 endControl)
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
        public BezierProperties GetModifiedCurveCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPosition, vector);
            Vector2 endPosition = mod(EndPosition, vector);
            Vector2 startControl = mod(StartControl, vector);
            Vector2 endControl = mod(EndControl, vector);
            return new BezierProperties(startPosition, endPosition, startControl, endControl);
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

    public class WaypointPathBezier : WaypointPathCreator
    {
        public override List<Vector2> GeneratePath(PathProperties properties, float stepSize = 0.5f)
        {
            BezierProperties beProperties;
            try { beProperties = (BezierProperties)properties; }
            catch (InvalidCastException e)
            {
                Debug.LogError(e.Message);
                return new List<Vector2>() { Vector2.zero };
            }

            if (beProperties.EndPosition == Vector2.zero)
                return new List<Vector2>() { Vector2.zero };

            if (stepSize <= 0.1f) stepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            List<Vector2> waypoints = new();
            if (beProperties.EndControl != Vector2.zero)
            {
                if (beProperties.StartControl != Vector2.zero)
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.CubicCurve(beProperties.StartPosition, beProperties.StartControl,
                            beProperties.StartControl, beProperties.EndPosition, t * stepSize / 100));
                    }
                }
                else
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.QuadraticCurve(beProperties.StartPosition, beProperties.StartControl,
                            beProperties.EndPosition, t * stepSize / 100));
                    }
                }
            }
            else
            {
                waypoints.Add(BezierCurve.Lerp(beProperties.StartPosition, beProperties.EndPosition, 1));
            }

            return waypoints;
        }
    }
}