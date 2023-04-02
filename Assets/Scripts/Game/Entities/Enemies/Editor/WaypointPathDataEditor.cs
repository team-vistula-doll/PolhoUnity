using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    SerializedProperty pathFormula;
    SerializedProperty length;
    SerializedProperty angle;
    SerializedProperty startControl;
    SerializedProperty endControl;
    SerializedProperty endPosition;

    List<Vector2> tempPath = new();
    int pathTypeSelection = 1;
    bool isReplace = false;

    private void OnEnable()
    {
        pathFormula = serializedObject.FindProperty("PathFormula");
        length = serializedObject.FindProperty("Length");
        angle = serializedObject.FindProperty("Angle");
        startControl = serializedObject.FindProperty("StartControl");
        endControl = serializedObject.FindProperty("EndControl");
        endPosition = serializedObject.FindProperty("EndPosition");
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
                pathData.SetWaypointPath(pathTypeSelection, !isReplace);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = pathData.CreateWaypointPath((isReplace) ? pathData.StartPosition : pathData.Path.Last(), pathTypeSelection);
        SceneView.RepaintAll();
    }

    bool isMousePressed = false;
    bool isEndControlEnabled = false;
    //bool isStartControlEnabled = false;

    DrawPathHandles drawPathHandles = new(false, false);
    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        Vector2 snap = Vector2.one * 0.2f;

        drawPathHandles.Draw(pathData, e, ref tempPath, pathTypeSelection, isReplace);
    }
}
