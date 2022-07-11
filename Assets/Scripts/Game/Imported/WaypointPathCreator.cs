using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using B83.ExpressionParser;

public static class WaypointPathCreator
//Given starting position to a selected function,
//generates List of waypoints (Vectors2) that WaypointWalkers will follow
{

    public static List<Vector2> GeneratePathFromExpression(Vector2 startPos, float length, String expression, float angle, float stepSize = 0.5f)
    {
        //Create expression parser and envaluate expression
        ExpressionParser parser = new ExpressionParser();
        Expression exp = parser.EvaluateExpression(expression);

        List<Vector2> waypoints = new List<Vector2>();

        angle = Mathf.Deg2Rad*angle;
        for(int i = 1; i*stepSize<length;i++)
        {
            exp.Parameters["x"].Value = i*stepSize; //Put x-val to x in expression

            Vector2 p = new Vector2(i*stepSize, (float) exp.Value); //point on a graph with origin (0,0)
            Vector2 rotatedPoint = new Vector2(p.x * Mathf.Cos(angle) - p.y * Mathf.Sin(angle),
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
    /// Generates a path from a cubic Bezier curve
    /// </summary>
    /// <param name="startPos">Start position of the curve</param>
    /// <param name="endPos">End position of the curve</param>
    /// <param name="startControl">Control point of the start position</param>
    /// <param name="endControl">Control point of the end position</param>
    /// <param name="numberOfPoints">Number of waypoints in the path</param>
    /// <returns>Waypoints of the path</returns>
    public static List<Vector2> GeneratePathFromCurve(Vector2 startPos, Vector2 endPos, Vector2 startControl, Vector2 endControl, ushort numberOfPoints = 10)
    {
        if (numberOfPoints <= 0)
        {
            numberOfPoints = 10;
            Debug.LogWarning("GeneratePathFromCubicCurve: numberOfPoints is 0, defaulting to 10");
        }

        List<Vector2> waypoints = new List<Vector2>();
        float step = 1f / numberOfPoints;
        for (float t = step; t <= 1f; t += step)
        {
            waypoints.Add(Bezier.CubicCurve(startPos, startControl, endControl, endPos, t));
        }

        return waypoints;
    }

    /// <summary>
    /// Generates a path from a quadratic Bezier curve
    /// </summary>
    /// <param name="startPos">Start position of the curve</param>
    /// <param name="endPos">End position of the curve</param>
    /// <param name="control">Control point of the curve</param>
    /// <param name="numberOfPoints">Number of waypoints in the path</param>
    /// <returns>Waypoints of the path</returns>
    public static List<Vector2> GeneratePathFromCurve(Vector2 startPos, Vector2 endPos, Vector2 control, ushort numberOfPoints = 10)
    {
        if (numberOfPoints <= 0)
        {
            numberOfPoints = 10;
            Debug.LogWarning("GeneratePathFromQuadraticCurve: numberOfPoints is 0, defaulting to 10");
        }

        List<Vector2> waypoints = new List<Vector2>();
        float step = 1f / numberOfPoints;
        for (float t = step; t <= 1f; t += step)
        {
            waypoints.Add(Bezier.QuadraticCurve(startPos, control, endPos, t));
        }

        return waypoints;
    }

}
