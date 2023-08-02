using Bezier;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{

    public class WaypointPathBezier : WaypointPathCreator
    {
        public Vector2 EndPosition = Vector2.zero;
        public Vector2 StartControl = Vector2.zero;
        public Vector2 EndControl = Vector2.zero;

        public void Init(Vector2 startPosition, Vector2 endPosition, Vector2 startControl, Vector2 endControl)
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
        public WaypointPathBezier GetModifiedCurveCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPosition, vector);
            Vector2 endPosition = mod(EndPosition, vector);
            Vector2 startControl = mod(StartControl, vector);
            Vector2 endControl = mod(EndControl, vector);
            var value = (WaypointPathBezier)ScriptableObject.CreateInstance(typeof(WaypointPathBezier));
            value.Init(startPosition, endPosition, startControl, endControl);
            return value;
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

        public override WaypointPathCreator GetNewAdjoinedPath(float percent)
        {
            if (percent < 0) percent = 0;
            if (percent > 1) percent = 1;
            Vector2 vector = BezierCurve.CubicCurve(StartPosition, StartControl, EndControl, EndPosition, percent);
            return GetModifiedCurveCopy(vector, (x, y) => x + y);
        }

        public override Vector2 GetEndVector() => EndPosition;

        public override List<Vector2> GeneratePath()
        {
            if (EndPosition == Vector2.zero)
                return new List<Vector2>() { Vector2.zero };

            if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            List<Vector2> waypoints = new();
            if (EndControl != Vector2.zero)
            {
                if (StartControl != Vector2.zero)
                {
                    for (int t = 1; t * StepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.CubicCurve(StartPosition, StartControl,
                            EndControl, EndPosition, t * StepSize / 100));
                    }
                }
                else
                {
                    for (int t = 1; t * StepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.QuadraticCurve(StartPosition, StartControl,
                            EndPosition, t * StepSize / 100));
                    }
                }
            }
            else
            {
                waypoints.Add(BezierCurve.Lerp(StartPosition, EndPosition, 1));
            }

            return waypoints;
        }
    }
}