using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    CurrentStageEnemies stageEnemies;
    SerializedProperty enemies;
    List<WaypointPathCreator> enemyPath { get { return stageEnemies.enemies[selectedEnemyIndex].Path; } }

    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";

    //List<Vector2> pathData = new() { Vector2.zero };
    int selectedEnemyIndex = 0;
    Vector2 startPosition = Vector2.zero;

    private void OnEnable()
    {
        if (stageEnemies == null) stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("enemies");

        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        serialData = new SerializedObject(data);

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");
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

        if (tempPath.arraySize > 1 && enemyPath.Count == 0)
        {
            data.PathTypeSelection = 0;
            serialData.Update();
            tempPath.ClearArray();
            WaypointPathEditorData.Options[1].SetPathCreator(new WaypointPathBezier());
            tempPath.arraySize++;
            WaypointPathCreator newExpression = new WaypointPathExpression();
            tempPath.GetArrayElementAtIndex(0).managedReferenceValue = newExpression;
            data.SelectedOption.SetPathCreator(newExpression);
        }

        if (pathData.transform.hasChanged)
        {
            data.SelectedOption.ObjectTransform = pathData.transform;
            data.SelectedOption.ConnectPaths(pathData.Path, 0);
            serializedObject.UpdateIfRequiredOrScript();
            data.SelectedOption.ConnectPaths(tempPath, 0);
            data.SelectedOption.SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            pathData.transform.hasChanged = false;
        }
        data.SelectedOption.ConnectPaths(tempPath, 0);

        startPosition = EditorGUILayout.Vector2Field("Start Position", startPosition);

        data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, enemyPath);

        selectedPathIndex.intValue -= data.SelectedOption.DeletePath(enemies, tempPath);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(pathTypeSelection, selectedPathIndex, tempPath);

        if (data.SelectedOption.PathOptions()) data.SelectedOption.ConnectPaths(data.TempPath, selectedPathIndex.intValue);

        if (data.SelectedOption.SetPath(enemies, isInsert, selectedPathIndex, tempPath))
        {
            pathTypeSelection.intValue = (int)WaypointPathEditorData.GetSelectedOption(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;

        //PathEditor.DrawPath(false, e);
    }
}
