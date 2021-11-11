using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using B83.ExpressionParser;

public static class WaypointPathCreator 
//Given starting position to a selected function, 
//generates List of waypoints (Vectors2) that WaypointWalkers will follow
{
 public static float stepSize = 0.5f; //Stepsize between x's for given function.

 public static List<Vector2> GeneratePathFromExpression(Vector2 startPos, float length, String expression, float angle)
 {
     //Create expression parser and envaluate expression
     ExpressionParser parser = new ExpressionParser();
     Expression exp = parser.EvaluateExpression(expression);

     List<Vector2> waypoints = new List<Vector2>();

     angle = Mathf.Deg2Rad*angle;
     for(int i = 0; i*stepSize<length;i++)
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

}
