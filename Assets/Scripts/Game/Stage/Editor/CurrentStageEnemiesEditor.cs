using EnemyClass;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    //int selectedEnemyIndex = 0;
    SerializedProperty serialEnemy;
    SerializedProperty id, enemyName, spawnTime, spawnPosition, path, spawnRepeats, fireable;
    Enemy enemy;
    Vector2 enemySpawnPosition;
    Vector2 enemyScale;
    Sprite enemySprite = null;
    List<bool> foldouts;
    int foldedOut = -1;

    private void OnEnable()
    {
        if (stageEnemies == null) stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("Enemies");
        foldouts = new();
        for (int i = 0; i < enemies.arraySize; i++) foldouts.Add(false);

        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        data.Init();
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

        if (GUILayout.Button("New enemy"))
        {
            Enemy newEnemy = new();
            if (stageEnemies.Enemies.Count > 0) newEnemy.SpawnTime = stageEnemies.Enemies[^1].SpawnTime;
            enemies.arraySize++;
            enemies.GetArrayElementAtIndex(enemies.arraySize - 1).managedReferenceValue = newEnemy;
            foldouts.Add(false);
        };

        for (int i = 0; i < enemies.arraySize; i++)
        {
            enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
            serialEnemy = enemies.GetArrayElementAtIndex(i);
            spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
            string label = "(" + spawnTime.floatValue.ToString("0.00") + ") Enemy " + (i+1);
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], label);
            if (!foldouts[i])
            {
                if (foldedOut == i)
                {
                    enemySprite = null;
                    id = spawnPosition = path = spawnRepeats = fireable = null;
                    foldedOut = -1;
                }
                continue;
            }
            else if (foldedOut != i)
            {
                for (int j = 0; j < foldouts.Count; j++) foldouts[j] = false;
                foldouts[i] = true;

                id = serialEnemy.FindPropertyRelative("ID");
                enemyName = serialEnemy.FindPropertyRelative("Name");
                spawnPosition = serialEnemy.FindPropertyRelative("SpawnPosition");
                path = serialEnemy.FindPropertyRelative("Path");
                spawnRepeats = serialEnemy.FindPropertyRelative("SpawnRepeats");
                fireable = serialEnemy.FindPropertyRelative("Fireable");

                GameObject enemyPrefab = PrefabUtility.LoadPrefabContents("Assets/Prefabs/Enemies/" + enemyName.stringValue + ".prefab");
                //{
                    enemySpawnPosition = enemyPrefab.transform.position;
                    enemySprite = enemyPrefab.GetComponent<SpriteRenderer>().sprite;
                    enemyScale = enemyPrefab.transform.localScale;
                //}
                PrefabUtility.UnloadPrefabContents(enemyPrefab);


                foldedOut = i;
            }


            if (tempPath.arraySize > 1 && path.arraySize == 0)
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

            EditorGUI.BeginChangeCheck();

            enemySpawnPosition = EditorGUILayout.Vector2Field("Spawn Position", enemySpawnPosition);

            data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, enemy.Path);

            selectedPathIndex.intValue -= data.SelectedOption.DeletePath(path, tempPath);
            if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

            EditorGUILayout.Space();
            PathEditor.PathTypes(pathTypeSelection, selectedPathIndex, tempPath);

            if (data.SelectedOption.PathOptions()) data.SelectedOption.ConnectPaths(data.TempPath, selectedPathIndex.intValue);

            if (data.SelectedOption.SetPath(path, isInsert, selectedPathIndex, tempPath))
            {
                pathTypeSelection.intValue = (int)WaypointPathEditorData.GetSelectedOption(
                        (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            }
            if (EditorGUI.EndChangeCheck())
            {
                enemies.GetArrayElementAtIndex(i).managedReferenceValue = serialEnemy;
            }
        }

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        if (foldedOut == -1) return;
        EventType e = Event.current.type;

        Vector2 screenPosition = HandleUtility.WorldToGUIPoint(enemySpawnPosition);
        Vector2 screenScale = enemySprite.rect.size / HandleUtility.GetHandleSize(enemySpawnPosition) * enemyScale;
        Handles.BeginGUI();
        GUI.DrawTexture(new Rect(
            screenPosition - screenScale / 2, screenScale),
            enemySprite.texture);
        Handles.EndGUI();

        EditorGUI.BeginChangeCheck();
        Vector2 newSpawnPosition = Handles.PositionHandle(enemySpawnPosition, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Moved enemy");
            enemySpawnPosition = newSpawnPosition;
            data.SelectedOption.StartPosition = enemySpawnPosition;
            data.SelectedOption.ConnectPaths(path, 0);
            serializedObject.UpdateIfRequiredOrScript();
            data.SelectedOption.ConnectPaths(tempPath, 0);
            data.SelectedOption.SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
        }

        //PathEditor.DrawPath(false, e);
    }
}
