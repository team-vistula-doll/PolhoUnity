using B83.ExpressionParser;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathExpression : WaypointPathCreator
    {
        [Delayed]
        public string PathFormula = "x";
        [Min(0f)]
        public float Length = 20;
        public float Angle = 0; //in degrees

        public WaypointPathExpression(Vector2 startPosition, string pathFormula, float length, float angle)
        {
            StartPosition = startPosition;
            PathFormula = pathFormula;
            Length = length;
            Angle = angle;
        }

        public override WaypointPathCreator GetNewAdjoinedPath(float percent)
        {
            Vector2 start = GetPointVector(percent * Length);
            WaypointPathExpression value = new(start, PathFormula, Length, Angle);
            return value;
        }

        static ExpressionParser parser = new();

        /// <summary>
        /// Returns a Vector2 of a point in an expression
        /// </summary>
        /// <param name="exp">Evaluated expression</param>
        /// <param name="x">The expression x input</param>
        /// <param name="angle">The rotation angle of the expression in degrees</param>
        /// <returns>Vector2 point = (<paramref name="x"/>, f(<paramref name="x"/>)) rotated by <paramref name="angle"/></returns>
        public Vector2 GetPointVector(Expression exp, float x, float angle)
        {
            angle *= Mathf.Deg2Rad;
            exp.Parameters["x"].Value = x; //Put x-val to x in expression

            Vector2 p = new(x, (float)exp.Value); //point on a graph with origin (0,0)
            if (angle != 0)
                p = new(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
                                            p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle));
            //Rotate point using rotation matrix
            return p;
        }

        public Vector2 GetPointVector(float x, float? angle = null)
        {
            Expression exp = parser.EvaluateExpression(PathFormula);
            return GetPointVector(exp, x, (angle != null) ? angle.Value : Angle);
        }

        public override Vector2 GetEndVector() => GetPointVector(Length);

        public override List<Vector2> GeneratePath()
        {
            if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            //Create expression parser and envaluate expression
            Expression exp = parser.EvaluateExpression(PathFormula);

            List<Vector2> waypoints = new();

            for (int i = 1; i * StepSize <= Length; i++)
            {
                Vector2 point = GetPointVector(exp, i * StepSize, Angle);
                waypoints.Add(point + StartPosition); //Translate point to originate from the startPos
            }

            Vector2 end = GetPointVector(exp, Length, Angle);
            if (waypoints.Any() && Vector2.Distance(waypoints.Last(), end) <= 0.1f)
                waypoints.RemoveAt(waypoints.Count - 1);
            waypoints.Add(end + StartPosition);

            return waypoints;
        }
    }
}