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
    SerializedProperty stepSize, tempPath, isReplace, pathTypeSelection;
    PathEditor PathEditor { get => data.Options.ElementAt(data.PathTypeSelection).Value; }

    WaypointPathData pathData;

    public void OnEnable()
    {
        pathData = target as WaypointPathData;
        data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        stepSize = serialData.FindProperty("StepSize");
        tempPath = serialData.FindProperty("TempPath");
        isReplace = serialData.FindProperty("IsReplace");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            pathTypeSelection.intValue = GUILayout.Toolbar(pathTypeSelection.intValue, data.Options.Keys.ToArray(), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        PathEditor.PathOptions();

        data.StepSize = EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = PathEditor.MakePath(data.IsReplace, data.StepSize);
                if (data.IsReplace || pathData.Path.Count() == 0) pathData.Path = path;
                else pathData.Path.AddRange(path);
            }
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();

        data.TempPath = PathEditor.MakePath(data.IsReplace, data.StepSize);
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        PathEditor.DrawPath(ref pathData.Path, e, ref data.TempPath, data.IsReplace);
    }
}
