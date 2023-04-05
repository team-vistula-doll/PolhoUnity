using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    BezierControlPoints bezier = new(Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
    ExpressionProperties expression = new("x", 20, 0);
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
                EditorGUILayout.PropertyField(pathFormula);
                EditorGUILayout.PropertyField(length);
                EditorGUILayout.PropertyField(angle);

                break;
            case 1:
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
                List<Vector2> path = WaypointPathCreator.CreateWaypointPath(
                    (!isReplace && pathData.Path.Count() != 0) ? pathData.Path.Last() : bezier.StartPosition);
                if (!isReplace) pathData.Path.AddRange(path);
                else pathData.Path = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = WaypointPathCreator.CreateWaypointPath(isReplace ? bezier.StartPosition : pathData.Path.Last());
        SceneView.RepaintAll();
    }

    DrawPathHandles drawPathHandles = new(false, false);
    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        drawPathHandles.Draw(pathData, e, ref tempPath, pathTypeSelection, isReplace);
    }
}
