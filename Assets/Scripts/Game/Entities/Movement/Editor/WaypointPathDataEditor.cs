using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isReplace, pathTypeSelection;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";
    PathEditor PathEditor { get => WaypointPathEditorData.Options[(int)data.PathTypeSelection]; }

    WaypointPathData pathData;
    SerializedProperty serialPathData;

    private void OnEnable()
    {
        pathData = target as WaypointPathData;
        serialPathData = serializedObject.FindProperty("Path");
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isReplace = serialData.FindProperty("IsReplace");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();

        PathEditor.SelectPath(ref selectedPathIndex, ref pathTypeSelection, ref pathData);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            pathTypeSelection.intValue = GUILayout.Toolbar(
                pathTypeSelection.intValue, Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        PathEditor.PathOptions();

        EditorGUILayout.BeginHorizontal();
        {
            isReplace.boolValue = EditorGUILayout.ToggleLeft("Replace", isReplace.boolValue);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = PathEditor.MakePath(data.IsReplace || serialPathData.arraySize == 1);

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

        data.TempPath = PathEditor.MakePath(data.IsReplace || serialPathData.arraySize == 1);

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        List<Vector2> vector2s = new();
        foreach (var path in pathData.Path) vector2s.AddRange(path.GeneratePath());
        PathEditor.DrawPath(in vector2s, e, in data);
    }
}
