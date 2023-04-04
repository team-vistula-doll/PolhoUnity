using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    public Vector2 StartPosition = Vector2.zero, StartControl, EndControl, EndPosition = Vector2.zero;

    [Delayed]
    public string PathFormula = "x";
    public float Length = 20, Angle;

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
                List<Vector2> path = CreateWaypointPath(
                    (!isReplace && pathData.Path.Count() != 0) ? pathData.Path.Last() : StartPosition);
                if (!isReplace) pathData.Path.AddRange(path);
                else pathData.Path = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        tempPath = CreateWaypointPath((isReplace) ? StartPosition : pathData.Path.Last());
        SceneView.RepaintAll();
    }

    DrawPathHandles drawPathHandles = new(false, false);
    public void OnSceneGUI()
    {
        WaypointPathData pathData = target as WaypointPathData;
        Event e = Event.current;

        drawPathHandles.Draw(pathData, e, ref tempPath, pathTypeSelection, isReplace);
    }

    List<Vector2> CreateWaypointPath(Vector2 startPos)
    {
        return pathTypeSelection switch
        {
            1 => WaypointPathCreator.GeneratePathFromCurve(startPos, EndPosition, StartControl, EndControl, StepSize),
            _ => WaypointPathCreator.GeneratePathFromExpression(startPos, Length, PathFormula, Angle, StepSize),
        };
    }
}
