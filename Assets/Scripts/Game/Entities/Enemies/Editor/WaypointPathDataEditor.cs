using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    //TODO: initialize these from somewhere
    BezierProperties bezier = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
    ExpressionProperties expression = new(Vector2.zero, "x", 20, 0);
    PathProperties properties;
    WaypointPathBezier pathBezier = new();
    WaypointPathExpression pathExpression = new();
    WaypointPathCreator creator;
    DrawBezier drawBezier = new(false, false);
    DrawExpression drawExpression = new();
    DrawPath drawPath;

    [Range(0.2f, 50f)]
    float stepSize = 0.5f;
    List<Vector2> tempPath = new();
    int pathTypeSelection = 1;
    bool isReplace = false;

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

            pathTypeSelection = GUILayout.Toolbar(pathTypeSelection, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection)
        {
            case 0:
                EditorGUILayout.DelayedTextField("Path formula", expression.PathFormula);
                EditorGUILayout.FloatField("Length", expression.Length);
                EditorGUILayout.FloatField("Angle", expression.Angle);
                properties = expression;
                drawPath = drawExpression;
                creator = pathExpression;

                break;
            case 1:
                EditorGUILayout.Vector2Field("End", bezier.EndPosition);
                EditorGUI.BeginDisabledGroup(bezier.EndPosition == Vector2.zero);
                    EditorGUILayout.Vector2Field("1st control", bezier.EndControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(bezier.EndControl == Vector2.zero);
                    EditorGUILayout.Vector2Field("2nd control", bezier.StartControl);
                EditorGUI.EndDisabledGroup();
                properties = bezier;
                drawPath = drawBezier;
                creator = pathBezier;

                break;
        }
        EditorGUILayout.Slider("Step size", stepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = creator.GeneratePath(
                    isReplace || pathData.Path.Count() == 0 ? properties : properties.GetNewAdjoinedPath(1), stepSize);
                if (!isReplace) pathData.Path.AddRange(path);
                else pathData.Path = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = creator.GeneratePath(isReplace ? properties : properties.GetNewAdjoinedPath(1), stepSize);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        drawPath.Draw(properties, pathData, e, ref tempPath, isReplace);
    }
}
