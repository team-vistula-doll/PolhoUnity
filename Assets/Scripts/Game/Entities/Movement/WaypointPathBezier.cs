using Bezier;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathBezier : WaypointPathCreator
    {
        public Vector2 EndPosition = Vector2.zero;
        public Vector2 EndControl = Vector2.zero;
        public Vector2 StartControl = Vector2.zero;

        public WaypointPathBezier() { }
        public WaypointPathBezier(Vector2 startPosition, Vector2 endPosition, Vector2 startControl, Vector2 endControl)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
            StartControl = startControl;
            EndControl = endControl;
        }

        public override WaypointPathCreator GetModifiedPathCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPosition, vector);
            Vector2 endPosition = mod(EndPosition, vector);
            Vector2 startControl = mod(StartControl, vector);
            Vector2 endControl = mod(EndControl, vector);
            WaypointPathBezier value = new(startPosition, endPosition, startControl, endControl);
            return value;
        }

        public override void ModifyPath(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
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
            return GetModifiedPathCopy(vector, (x, y) => x + y);
        }

        public override Vector2? GetVectorAt(float percent) => BezierCurve.CubicCurve(StartPosition, StartControl,
                            EndControl, EndPosition, percent);

        public override List<Vector2> GeneratePath()
        {
            float step = StepSize * 10;
            if (EndPosition == Vector2.zero)
                return new List<Vector2>() { Vector2.zero };

            //if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            List<Vector2> waypoints = new();
            if (EndControl - StartPosition != Vector2.zero)
            {
                if (StartControl - StartPosition != Vector2.zero)
                {
                    for (int t = 1; t * step <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.CubicCurve(StartPosition, StartControl,
                            EndControl, EndPosition, t * step / 100));
                    }
                }
                else
                {
                    for (int t = 1; t * step <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.QuadraticCurve(StartPosition, EndControl,
                            EndPosition, t * step / 100));
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