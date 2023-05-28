using B83.ExpressionParser;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionProperties : PathProperties
    {
        public string PathFormula;
        public float Length;
        public float Angle;

        public ExpressionProperties(Vector2 startPos, string pathFormula, float length, float angle)
        {
            StartPosition = startPos;
            PathFormula = pathFormula;
            Length = length;
            Angle = angle;
        }

        public override PathProperties GetNewAdjoinedPath(float percent)
        {
            Vector2 start = WaypointPathExpression.GetPointVector(this, percent * Length);
            return new ExpressionProperties(start, PathFormula, Length, Angle);
        }
    }
    public class WaypointPathExpression : WaypointPathCreator
    {
        static ExpressionParser parser = new();

        public static Vector2 GetPointVector(Expression exp, float x, float angle)
        {
            exp.Parameters["x"].Value = x; //Put x-val to x in expression

            Vector2 p = new(x, (float)exp.Value); //point on a graph with origin (0,0)
            if (angle != 0)
                p = new(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
                                            p.x * Mathf.Sin(angle) + p.y * Mathf.Cos(angle));
            //Rotate point using rotation matrix
            return p;
        }

        public static Vector2 GetPointVector(ExpressionProperties properties, float x)
        {
            Expression exp = parser.EvaluateExpression(properties.PathFormula);
            return GetPointVector(exp, x, properties.Angle);
        }

        public override List<Vector2> GeneratePath(PathProperties properties, float stepSize = 0.5f)
        {
            ExpressionProperties exProperties;
            try { exProperties = (ExpressionProperties)properties; }
            catch (InvalidCastException e)
            {
                Debug.LogError(e.Message);
                return new List<Vector2>() { Vector2.zero };
            }

            //Create expression parser and envaluate expression
            Expression exp = parser.EvaluateExpression(exProperties.PathFormula);

            List<Vector2> waypoints = new();

            exProperties.Angle *= Mathf.Deg2Rad;
            for (int i = 1; i * stepSize <= exProperties.Length; i++)
            {
                Vector2 point = GetPointVector(exp, i * stepSize, exProperties.Angle);
                waypoints.Add(point + properties.StartPosition); //Translate point to originate from the startPos
            }

            return waypoints;
        }
    }
}