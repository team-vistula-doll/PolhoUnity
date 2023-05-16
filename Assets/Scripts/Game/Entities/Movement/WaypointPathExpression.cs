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
    }
    public class WaypointPathExpression : WaypointPathCreator
    {
        ExpressionParser parser = new();
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
            for (int i = 1; i * stepSize < exProperties.Length; i++)
            {
                exp.Parameters["x"].Value = i * stepSize; //Put x-val to x in expression

                Vector2 p = new(i * stepSize, (float)exp.Value); //point on a graph with origin (0,0)
                Vector2 rotatedPoint = new(p.x * Mathf.Cos(exProperties.Angle) - p.y * Mathf.Sin(exProperties.Angle),
                                                p.x * Mathf.Sin(exProperties.Angle) + p.y * Mathf.Cos(exProperties.Angle));
                //Rotate point using rotation matrix
                waypoints.Add(rotatedPoint + properties.StartPosition); //Translate rotatedPoint to originate from the startPos
            }

            return waypoints;
        }
    }
}