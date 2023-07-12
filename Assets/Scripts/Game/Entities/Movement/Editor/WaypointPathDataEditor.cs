using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty stepSize, isReplace, pathTypeSelection;
    PathEditor PathEditor { get => data.Options.ElementAt(data.PathTypeSelection).Value; }

    WaypointPathData pathData;
    SerializedProperty serialPathData;

    private void OnEnable()
    {
        pathData = target as WaypointPathData;
        serialPathData = serializedObject.FindProperty("Path");
        data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        stepSize = serialData.FindProperty("StepSize");
        isReplace = serialData.FindProperty("IsReplace");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            pathTypeSelection.intValue = GUILayout.Toolbar(
                pathTypeSelection.intValue, data.Options.Keys.ToArray(), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        PathEditor.PathOptions();

        EditorGUILayout.PropertyField(stepSize);

        EditorGUILayout.BeginHorizontal();
        {
            isReplace.boolValue = EditorGUILayout.ToggleLeft("Replace", isReplace.boolValue);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = PathEditor.MakePath(data.IsReplace || serialPathData.arraySize == 1, data.StepSize);

                //Setting the new path in the edited object through serializedObject
                if (data.IsReplace || serialPathData.arraySize == 1) serialPathData.ClearArray();
                foreach (Vector2 v in path)
                {
                    serialPathData.arraySize++;
                    serialPathData.GetArrayElementAtIndex(serialPathData.arraySize - 1).vector2Value = v;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        data.TempPath = PathEditor.MakePath(data.IsReplace || serialPathData.arraySize == 1, data.StepSize);

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        PathEditor.DrawPath(in pathData.Path, e, in data);
    }
}
