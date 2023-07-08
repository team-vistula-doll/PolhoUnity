using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class ExpressionEditor : PathEditor
    {
        WaypointPathExpression pathExpression;

        public void OnEnable()
        {
            pathExpression = (WaypointPathExpression)ScriptableObject.CreateInstance(typeof(WaypointPathExpression));
        }

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

        new public void DrawPath(ref List<Vector2> pathData, Event e, ref List<Vector2> tempPath, bool isReplace)
        {
            base.DrawPath(ref pathData, e, ref tempPath, isReplace);

            if (!isReplace) pathExpression.Properties.StartPosition = WaypointPathExpression.GetPointVector(pathExpression.Properties, pathExpression.Properties.Length);

            tempPath = pathExpression.GeneratePath(pathExpression.Properties);
        }
    }
}