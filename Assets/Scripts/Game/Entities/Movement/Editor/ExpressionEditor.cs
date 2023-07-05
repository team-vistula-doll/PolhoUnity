using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        WaypointPathExpression pathExpression = new();
        DrawExpression drawExpression = new();
        public override void PathOptions()
        {
            pathExpression.Properties.PathFormula = EditorGUILayout.DelayedTextField("Path formula", pathExpression.Properties.PathFormula);
            pathExpression.Properties.Length = EditorGUILayout.FloatField("Length", pathExpression.Properties.Length);
            pathExpression.Properties.Angle = EditorGUILayout.FloatField("Angle", pathExpression.Properties.Angle);
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (isReplace) pathExpression.Properties = (ExpressionProperties)pathExpression.Properties.GetNewAdjoinedPath(1);
            return pathExpression.GeneratePath(stepSize);
        }

        public override void DrawPath(ref WaypointPathData pathData, Event e, ref WaypointPathData tempPath, bool isReplace)
        {
            drawExpression.Draw(pathExpression.Properties, ref pathData, e, ref tempPath, isReplace);
        }
    }
}