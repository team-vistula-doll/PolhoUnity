using Bezier;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathBezier : WaypointPathCreator
    {
        public Vector2 EndPosition = Vector2.zero;
        public Vector2 EndControl = Vector2.zero;
        public Vector2 StartControl = Vector2.zero;

        public WaypointPathBezier() { }

        /// <summary>
        /// All points use absolute positioning
        /// </summary>
        public WaypointPathBezier(Waypoint startPoint, Vector2 endPosition, Vector2 startControl, Vector2 endControl)
        {
            StartPoint = startPoint;
            EndPosition = endPosition;
            StartControl = startControl;
            EndControl = endControl;
        }

        public override WaypointPathCreator GetModifiedPathCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPoint.Position, vector);
            Waypoint startPoint = new(startPosition, StartPoint.Speed, StartPoint.Acceleration);
            Vector2 endPosition = mod(EndPosition, vector);
            Vector2 startControl = mod(StartControl, vector);
            Vector2 endControl = mod(EndControl, vector);
            WaypointPathBezier copy = new(startPoint, endPosition, startControl, endControl);
            return copy;
        }

        public override void ModifyPath(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            StartPoint.Position = mod(StartPoint.Position, vector);
            EndPosition = mod(EndPosition, vector);
            StartControl = mod(StartControl, vector);
            EndControl = mod(EndControl, vector);
        }

        public override WaypointPathCreator GetNewAdjoinedPath(float percent)
        {
            if (percent < 0) percent = 0;
            if (percent > 1) percent = 1;
            Vector2 vector = BezierCurve.CubicCurve(StartPoint.Position, StartControl, EndControl, EndPosition, percent);
            return GetModifiedPathCopy(vector, (x, y) => x + y);
        }

        public override Vector2? GetVectorAt(float percent)
        {
            if (EndControl - StartPoint.Position == Vector2.zero)
            {
                return BezierCurve.Lerp(StartPoint.Position, EndPosition, percent);
            }
            if (StartControl - StartPoint.Position == Vector2.zero)
            {
                return BezierCurve.QuadraticCurve(StartPoint.Position, EndControl, EndPosition, percent);
            }
            return BezierCurve.CubicCurve(StartPoint.Position, StartControl, EndControl, EndPosition, percent);
        }

        public void AddWaypointAt(float percent, float? speed = null, float? acceleration = null)
        {
            KeyWaypoints[percent] = new(GetVectorAt(percent).Value, speed, acceleration);
        }

        public override List<Waypoint> GeneratePath()
        {
            if (EndPosition == Vector2.zero)
                return new List<Waypoint>() { new(Vector2.zero, null, null) };

            //if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            var enumerator = KeyWaypoints.GetEnumerator();
            List<Waypoint> waypoints = new();
            if (Vector2.Distance(EndControl, StartPoint.Position) < 0.0005)
            {
                while (enumerator.MoveNext())
                {
                    waypoints.Add(enumerator.Current.Value);
                }
                if (Math.Abs(KeyWaypoints.LastOrDefault().Key - 1) > 0.005)
                    waypoints.Add(new(EndPosition, null, null));
            }
            else
            {
                bool keyWaypointExists = enumerator.MoveNext();
                bool keyWaypointIsClose = false;
                for (float t = 0.01f; t * StepSize <= 1; t += 0.01f)
                {
                    if (keyWaypointExists)
                    {
                        do
                        {
                            if (enumerator.Current.Key <= t)
                            {
                                waypoints.Add(enumerator.Current.Value);
                                if (Math.Abs(enumerator.Current.Key - t) > 0.005) keyWaypointIsClose = true;
                            }
                            else break;
                            keyWaypointExists = enumerator.MoveNext();
                        } while (keyWaypointExists);
                    }
                    if (!keyWaypointIsClose) 
                    {
                        if (Vector2.Distance(StartControl, StartPoint.Position) < 0.0005)
                            waypoints.Add(new(BezierCurve.QuadraticCurve(StartPoint.Position, EndControl,
                                EndPosition, t * StepSize), null, null));
                        else
                            waypoints.Add(new(BezierCurve.CubicCurve(StartPoint.Position, StartControl,
                                EndControl, EndPosition, t * StepSize), null, null));
                    }
                    keyWaypointIsClose = false;
                }
            }

            return waypoints;
        }
    }
}