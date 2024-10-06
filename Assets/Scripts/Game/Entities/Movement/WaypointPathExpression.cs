using B83.ExpressionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static B83.ExpressionParser.ExpressionParser;

namespace WaypointPath
{
    public class WaypointPathExpression : WaypointPathCreator
    {
        [Delayed]
        public string PathFormula = "x";
        [Min(0f)]
        public float Length = 20;
        public float Angle = 0; //in degrees

        public WaypointPathExpression() { }
        public WaypointPathExpression(Waypoint startPoint, string pathFormula, float length, float angle)
        {
            StartPoint = startPoint;
            PathFormula = pathFormula;
            Length = length;
            Angle = angle;
        }

        public override WaypointPathCreator GetModifiedPathCopy(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            Vector2 startPosition = mod(StartPoint.Position, vector);
            Waypoint startPoint = new(startPosition, StartPoint.Speed, StartPoint.Acceleration);
            WaypointPathExpression value = new(startPoint, PathFormula, Length, Angle);
            return value;
        }

        public override WaypointPathCreator GetNewAdjoinedPath(float percent)
        {
            Vector2 start = (Vector2)GetPointVector(percent * Length, Angle);
            Waypoint startPoint = new(start, StartPoint.Speed, StartPoint.Acceleration);
            WaypointPathExpression value = new(startPoint, PathFormula, Length, Angle);
            return value;
        }

        public override void ModifyPath(Vector2 vector, Func<Vector2, Vector2, Vector2> mod)
        {
            StartPoint.Position = mod(StartPoint.Position, vector);
        }

        static ExpressionParser parser = new();

        /// <summary>
        /// Returns a Vector2 of a point in an expression
        /// </summary>
        /// <param name="exp">Evaluated expression</param>
        /// <param name="x">The expression x input</param>
        /// <param name="angle">The rotation angle of the expression in degrees</param>
        /// <returns>Vector2 point = (<paramref name="x"/>, f(<paramref name="x"/>)) rotated by <paramref name="angle"/>
        /// and moved by the Start position</returns>
        public Vector2 GetPointVector(Expression exp, float x, float angle)
        {
            exp.Parameters["x"].Value = x; //Put x-val to x in expression

            Vector2 p = new(x, (float)exp.Value); //point on a graph with origin (0,0)
            angle *= Mathf.Deg2Rad;
            if (angle != 0)
                p = new(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
                                            p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle));
            //Rotate point using rotation matrix

            return p + StartPoint.Position; //Translate point to originate from the startPos
        }

        public Vector2? GetPointVector(float x, float? angle = null)
        {
            Expression exp;
            try
            {
                exp = parser.EvaluateExpression(PathFormula);
            } catch (ParseException) { return null; }
            return GetPointVector(exp, x, (angle != null) ? angle.Value : Angle);
        }

        public override Vector2? GetVectorAt(float percent) => GetPointVector(percent * Length);

        public override List<Vector2> GeneratePath()
        {
            if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing
            Expression exp;

            //Create expression parser and evaluate expression
            try
            {
                exp = parser.EvaluateExpression(PathFormula);
            } catch (ParseException) { return new(); }

            List<Vector2> waypoints = new();

            for (int i = 1; i * StepSize <= Length; i++)
            {
                Vector2 point = GetPointVector(exp, i * StepSize, Angle);
                waypoints.Add(point);
            }

            Vector2 end = GetPointVector(exp, Length, Angle);
            if (waypoints.Count != 0 && Vector2.Distance(waypoints.Last(), end) <= 0.1f)
                waypoints.RemoveAt(waypoints.Count - 1);
            waypoints.Add(end);

            return waypoints;
        }
    }
}