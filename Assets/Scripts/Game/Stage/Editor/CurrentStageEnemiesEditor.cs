using EnemyStruct;
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

    WaypointPathEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";

    int selectedEnemyIndex = 0;
    Enemy enemy;
    Vector2 enemySpawnPosition;
    Sprite enemySprite = null;
    List<WaypointPathCreator> enemyPath { get { return stageEnemies.enemies[selectedEnemyIndex].Path; } }
    List<bool> foldouts;
    int foldedOut = -1;

    private void OnEnable()
    {
        if (stageEnemies == null) stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("enemies");
        foldouts = new();
        for (int i = 0; i < enemies.arraySize; i++) foldouts.Add(false);

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

        EditorGUILayout.LabelField("List size: " + enemies.arraySize);
        EditorGUILayout.Space();

        for (int i = 0; i < enemies.arraySize; i++)
        {
            enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
            string label = "(" + enemy.SpawnTime.ToString("0.00") + ") Enemy " + (i+1);
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], label);
            if (!foldouts[i])
            {
                if (foldedOut == i)
                {
                    enemySprite = null;
                    foldedOut = -1;
                }
                continue;
            }
            else if (foldedOut != i)
            {
                for (int j = 0; j < foldouts.Count; j++) foldouts[j] = false;
                foldouts[i] = true;

                GameObject enemyPrefab = PrefabUtility.LoadPrefabContents("Prefabs/Enemies/" + enemy.Name);
                //{
                    enemySpawnPosition = enemyPrefab.transform.position;
                    enemySprite = enemyPrefab.GetComponent<SpriteRenderer>().sprite;
                //}
                PrefabUtility.UnloadPrefabContents(enemyPrefab);

                foldedOut = i;
            }

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
            data.SelectedOption.ConnectPaths(tempPath, 0);

            enemySpawnPosition = EditorGUILayout.Vector2Field("Start Position", enemySpawnPosition);

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
        }

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;

        GUI.DrawTexture(new Rect(enemySpawnPosition, enemySprite.textureRect.size), enemySprite.texture);
        EditorGUI.BeginChangeCheck();
        Vector2 newSpawnPosition = Handles.PositionHandle(enemySpawnPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Moved enemy");
            enemySpawnPosition = newSpawnPosition;
            data.SelectedOption.StartPosition = enemySpawnPosition;
            data.SelectedOption.ConnectPaths(enemyPath, 0);
            serializedObject.UpdateIfRequiredOrScript();
            data.SelectedOption.ConnectPaths(tempPath, 0);
            data.SelectedOption.SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
        }

        //PathEditor.DrawPath(false, e);
    }
}
