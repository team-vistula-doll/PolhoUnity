using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;
using System;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data = new();
    PathProperties properties { get { return data.Creator.Properties; } set { data.Creator.Properties = value; } }

    string[] pathOptions = new string[] { "Function", "Bezier" };
    private void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        WaypointPathData pathData = target as WaypointPathData;

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            data.PathTypeSelection = GUILayout.Toolbar(data.PathTypeSelection, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (data.PathTypeSelection)
        {
            case 0:
                data.Creator = data.PathExpression;
                (ExpressionProperties)properties.PathFormula = EditorGUILayout.DelayedTextField("Path formula", data.Expression.PathFormula);
                data.Expression.Length = EditorGUILayout.FloatField("Length", data.Expression.Length);
                data.Expression.Angle = EditorGUILayout.FloatField("Angle", data.Expression.Angle);
                data.Properties = data.Expression;
                data.DrawPath = data.DrawExpression;

                break;
            case 1:
                data.Bezier.EndPosition = EditorGUILayout.Vector2Field("End", data.Bezier.EndPosition);
                EditorGUI.BeginDisabledGroup(data.Bezier.EndPosition == Vector2.zero);
                    data.Bezier.EndControl = EditorGUILayout.Vector2Field("1st control", data.Bezier.EndControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(data.Bezier.EndControl == Vector2.zero);
                    data.Bezier.StartControl = EditorGUILayout.Vector2Field("2nd control", data.Bezier.StartControl);
                EditorGUI.EndDisabledGroup();
                data.Properties = data.Bezier;
                data.DrawPath = data.DrawBezier;
                data.Creator = data.PathBezier;

                break;
        }
        EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                data.Creator.Properties = data.IsReplace || pathData.Path.Count() == 0 ? data.Properties : data.Properties.GetNewAdjoinedPath(1);
                List<Vector2> path = data.Creator.GeneratePath(data.StepSize);
                if (data.IsReplace || pathData.Path.Count() == 0) pathData.Path = path;
                else pathData.Path.AddRange(path);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();


        data.TempPath = data.Creator.GeneratePath(data.IsReplace ? data.Properties : data.Properties.GetNewAdjoinedPath(1), data.StepSize);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        data.DrawPath.Draw(data.Properties, pathData, e, ref data.TempPath, data.IsReplace);
    }
}
