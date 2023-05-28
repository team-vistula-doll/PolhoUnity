using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

//TODO: Make a class WaypointPathManager that modifies paths so it's possible from within the game and not just the editor
//move some of the code from here
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
    public float StepSize = 0.5f;

    List<Vector2> tempPath = new();
    int pathTypeSelection = 1;
    bool isReplace = false;

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
            string[] pathOptions = new string[] { "Function", "Bezier" };

            pathTypeSelection = GUILayout.Toolbar(pathTypeSelection, pathOptions, EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        switch (pathTypeSelection)
        {
            case 0:
                properties = expression;
                drawPath = drawExpression;
                creator = pathExpression;
                EditorGUILayout.PropertyField(pathFormula);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(angle);

                break;
            case 1:
                properties = bezier;
                drawPath = drawBezier;
                creator = pathBezier;
                EditorGUILayout.PropertyField(endPosition);
                EditorGUI.BeginDisabledGroup(endPosition.vector2Value ==  Vector2.zero);
                    EditorGUILayout.PropertyField(endControl);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(endControl.vector2Value == Vector2.zero);
                    EditorGUILayout.PropertyField(startControl);
                EditorGUI.EndDisabledGroup();

                break;
        }
        SerializedProperty stepSize = serializedObject.FindProperty("StepSize");
        EditorGUILayout.PropertyField(stepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace = EditorGUILayout.ToggleLeft("Replace", isReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = creator.GeneratePath(
                    isReplace || pathData.Path.Count() == 0 ? properties : properties.GetNewAdjoinedPath(1));
                if (!isReplace) pathData.Path.AddRange(path);
                else pathData.Path = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = creator.GeneratePath(isReplace ? properties : properties.GetNewAdjoinedPath(1));
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        drawPath.Draw(properties, pathData, e, ref tempPath, isReplace);
    }
}
