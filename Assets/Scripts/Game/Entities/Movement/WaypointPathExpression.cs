using B83.ExpressionParser;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class WaypointPathExpression : WaypointPathCreator
    {
        [Delayed]
        public string PathFormula = "x";
        public float Length = 20;
        public float Angle = 0;
        public void Init(Vector2 startPosition, string pathFormula, float length, float angle)
        {
            StartPosition = startPosition;
            PathFormula = pathFormula;
            Length = length;
            Angle = angle;
        }

        public override WaypointPathCreator GetNewAdjoinedPath(float percent)
        {
            Vector2 start = GetPointVector(percent * Length);
            WaypointPathExpression value = (WaypointPathExpression)ScriptableObject.CreateInstance(typeof(WaypointPathExpression));
            value.Init(start, PathFormula, Length, Angle);
            return value;
        }

        static ExpressionParser parser = new();

        public Vector2 GetPointVector(Expression exp, float x, float angle)
        {
            exp.Parameters["x"].Value = x; //Put x-val to x in expression

            Vector2 p = new(x, (float)exp.Value); //point on a graph with origin (0,0)
            if (angle != 0)
                p = new(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
                                            p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle));
            //Rotate point using rotation matrix
            return p;
        }

        public Vector2 GetPointVector(float x)
        {
            Expression exp = parser.EvaluateExpression(PathFormula);
            return GetPointVector(exp, x, Angle);
        }

        public override Vector2 GetEndVector() => GetPointVector(Length);

        public override List<Vector2> GeneratePath()
        {
            if (StepSize < 0.2f) StepSize = 0.2f; //Prevent too many waypoints and Unity freezing

            //Create expression parser and envaluate expression
            Expression exp = parser.EvaluateExpression(PathFormula);

            List<Vector2> waypoints = new();

            var angle = Angle * Mathf.Deg2Rad;
            for (int i = 1; i * StepSize <= Length; i++)
            {
                Vector2 point = GetPointVector(exp, i * StepSize, angle);
                waypoints.Add(point + StartPosition); //Translate point to originate from the startPos
            }

            return waypoints;
        }
    }
}