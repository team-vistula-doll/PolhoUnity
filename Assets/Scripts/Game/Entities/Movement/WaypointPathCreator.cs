using System.Collections.Generic;
using UnityEngine;
using B83.ExpressionParser;
using Bezier;

namespace WaypointPath
{
    public static class WaypointPathCreator
    //Given starting position to a selected function,
    //generates List of waypoints (Vectors2) that WaypointWalkers will follow
    {

        public static List<Vector2> GeneratePathFromExpression(Vector2 startPos, float length, string expression, float angle, float stepSize = 0.5f)
        {
            //Create expression parser and envaluate expression
            ExpressionParser parser = new();
            Expression exp = parser.EvaluateExpression(expression);

            List<Vector2> waypoints = new();

            angle = Mathf.Deg2Rad * angle;
            for (int i = 1; i * stepSize < length; i++)
            {
                exp.Parameters["x"].Value = i * stepSize; //Put x-val to x in expression

                Vector2 p = new(i * stepSize, (float)exp.Value); //point on a graph with origin (0,0)
                Vector2 rotatedPoint = new(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
                                                p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle));
                //Rotate point using rotation matrix
                waypoints.Add(rotatedPoint + startPos); //Translate rotatedPoint to originate from the startPos
            }

            return waypoints;
        }

        /// <summary>
        /// Creates a Vector2 from its length and angle
        /// </summary>
        /// <param name="length">Magnitude of the vector; if <= 0 defaults to 1</param>
        /// <param name="angle">Direction of the vector in degrees</param>
        public static Vector2 CreateVector2(float length, float angle)
        {
            if (length <= 0) length = 1;
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * length;
        }

        /// <summary>
        /// Generates a path from a Bezier curve
        /// </summary>
        /// <param name="startPos">Start position of the curve</param>
        /// <param name="endPos">End position of the curve</param>
        /// <param name="startControl">Control point of the start position/main control point for quadratic curve</param>
        /// <param name="endControl">Control point of the end position</param>
        /// <returns>Waypoints of the path</returns>
        public static List<Vector2> GeneratePathFromCurve(Vector2 startPos, Vector2 endPos, Vector2? startControl = null, Vector2? endControl = null, float stepSize = 0.5f)
        {
            if (endPos == Vector2.zero)
                return new List<Vector2>() { Vector2.zero };

            if (stepSize <= 0.1f) stepSize = 0.5f; //Prevent too many waypoints and game freezing

            Vector2 startCtrl = startControl ?? Vector2.zero,
                endCtrl = endControl ?? Vector2.zero;

            List<Vector2> waypoints = new();
            if (endCtrl != Vector2.zero)
            {
                if (startCtrl != Vector2.zero)
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.CubicCurve(startPos, startCtrl, endCtrl, endPos, t * stepSize / 100));
                    }
                }
                else
                {
                    for (int t = 1; t * stepSize <= 100; t++)
                    {
                        waypoints.Add(BezierCurve.QuadraticCurve(startPos, endCtrl, endPos, t * stepSize / 100));
                    }
                }
            }
            else
            {
                waypoints.Add(BezierCurve.Lerp(startPos, endPos, 1));
            }

            return waypoints;
        }
    }
}