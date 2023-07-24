using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    CurrentStageEnemies stageEnemies;
    SerializedProperty enemies;

    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty stepSize, isInsert, pathTypeSelection;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";
    PathEditor PathEditor { get { return WaypointPathEditorData.Options.ElementAt((int)data.PathTypeSelection); } }

    List<Vector2> pathData = new() { Vector2.zero };
    Vector2 startPosition = Vector2.zero;

    private void OnEnable()
    {
        stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("enemies");

        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        stepSize = serialData.FindProperty("StepSize");
        isInsert = serialData.FindProperty("IsInsert");
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

        startPosition = EditorGUILayout.Vector2Field("Start Position", startPosition);
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Label("Path type: ");

            pathTypeSelection.intValue = GUILayout.Toolbar(
                pathTypeSelection.intValue, Enum.GetNames(typeof(PathType)), EditorStyles.radioButton);
            //data.PathTypeSelection = GUILayout.Toolbar(data.PathTypeSelection, data.Options.Keys.ToArray(), EditorStyles.radioButton);
        }
        EditorGUILayout.EndHorizontal();

        PathEditor.PathOptions();

        EditorGUILayout.PropertyField(stepSize);
        //data.StepSize = EditorGUILayout.Slider("Step size", data.StepSize, 0.2f, 50f);

        EditorGUILayout.BeginHorizontal();
        {
            isInsert.boolValue = EditorGUILayout.ToggleLeft("Replace", isInsert.boolValue);
            //data.IsReplace = EditorGUILayout.ToggleLeft("Replace", data.IsReplace);

            if (GUILayout.Button("Set path"))
            {
                List<Vector2> path = PathEditor.MakePath(data.IsInsert || pathData.Count() == 1);
                if (data.IsInsert || pathData.Count() == 1) pathData = path;
                else pathData.AddRange(path);

                
            }
        }
        EditorGUILayout.EndHorizontal();

        data.TempPath = PathEditor.MakePath(data.IsInsert || pathData.Count() == 1);

        serializedObject.ApplyModifiedProperties();
        serialData.ApplyModifiedProperties();
        SceneView.RepaintAll();

        //base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        Event e = Event.current;

        PathEditor.DrawPath(in pathData, e, in data);
    }
}
