using B83.ExpressionParser;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public struct ExpressionProperties
    {
        [Delayed]
        public string PathFormula;
        public float Length;
        public float Angle;

        public ExpressionProperties(string pathFormula, float length, float angle)
        {
            PathFormula = pathFormula;
            Length = length;
            Angle = angle;
        }
    }
    public class WaypointPathExpression : WaypointPathCreator
    {
        ExpressionProperties expression = new("x", 20, 0);
        public override List<Vector2> GeneratePath(Vector2 startPos, float stepSize = 0.5f)
        {
            //Create expression parser and envaluate expression
            ExpressionParser parser = new();
            Expression exp = parser.EvaluateExpression(expression.PathFormula);

            List<Vector2> waypoints = new();

            expression.Angle = Mathf.Deg2Rad * expression.Angle;
            for (int i = 1; i * stepSize < expression.Length; i++)
            {
                exp.Parameters["x"].Value = i * stepSize; //Put x-val to x in expression

                Vector2 p = new(i * stepSize, (float)exp.Value); //point on a graph with origin (0,0)
                Vector2 rotatedPoint = new(p.x * Mathf.Cos(expression.Angle) - p.y * Mathf.Sin(expression.Angle),
                                                p.x * Mathf.Sin(expression.Angle) + p.y * Mathf.Cos(expression.Angle));
                //Rotate point using rotation matrix
                waypoints.Add(rotatedPoint + startPos); //Translate rotatedPoint to originate from the startPos
            }

            return waypoints;
        }
    }
}