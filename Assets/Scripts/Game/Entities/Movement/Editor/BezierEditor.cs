using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WaypointPath
{
    public class BezierEditor : PathEditor
    {
        WaypointPathBezier pathBezier = new();
        DrawBezier drawBezier = new(false, false);
        public override void PathOptions()
        {
            pathBezier.Properties.EndPosition = EditorGUILayout.Vector2Field("End", pathBezier.Properties.EndPosition);
            EditorGUI.BeginDisabledGroup(pathBezier.Properties.EndPosition == Vector2.zero);
                pathBezier.Properties.EndControl = EditorGUILayout.Vector2Field("1st control", pathBezier.Properties.EndControl);
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(pathBezier.Properties.EndControl == Vector2.zero);
                pathBezier.Properties.StartControl = EditorGUILayout.Vector2Field("2nd control", pathBezier.Properties.StartControl);
            EditorGUI.EndDisabledGroup();
        }

        public override List<Vector2> MakePath(bool isReplace, float stepSize)
        {
            if (isReplace) pathBezier.Properties = (BezierProperties)pathBezier.Properties.GetNewAdjoinedPath(1);
            return pathBezier.GeneratePath(stepSize);
        }

        public override void DrawPath(ref WaypointPathData pathData, Event e, ref WaypointPathData tempPath, bool isReplace)
        {
            drawBezier.Draw(pathBezier.Properties, ref pathData, e, ref tempPath, isReplace);
        }
    }
}