using EnemyClass;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using WaypointPath;
using static SerializedObjectUtility.SerializedObjectUtility;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    CurrentStageEnemies stageEnemies;
    SerializedProperty enemies;
    SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path/*, spawnRepeats, fireable*/;

    CurrentStageEnemiesEditorData data;
    SerializedObject serialData;
    SerializedProperty prefabID, selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    SerializedProperty foldouts, foldedOut, idIncrement;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";

    [SerializeReference]
    List<SingleEnemyEditor> enemyEditors = new();
    [SerializeField]
    Enemy enemy, selectedEnemy = null;
    [SerializeField]
    bool isIncorrectPrefab = false;
    bool wasTextureMoved = false;

    private void OnEnable()
    {
        if (stageEnemies == null) stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("Enemies");

        if (data == null) data = (CurrentStageEnemiesEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(CurrentStageEnemiesEditorData));
        if (data == null) data = (CurrentStageEnemiesEditorData)ScriptableObject.CreateInstance(typeof(CurrentStageEnemiesEditorData));
        data.Init();
        serialData = new SerializedObject(data);
        for (int i = 0; i < enemies.arraySize; i++) data.Foldouts.Add(false);
        if (stageEnemies.Enemies.Count > 0) data.IDIncrement = stageEnemies.Enemies[^1].ID + 1;

        prefabID = serialData.FindProperty("PrefabID");
        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");

        foldouts = serialData.FindProperty("Foldouts");
        foldedOut = serialData.FindProperty("FoldedOut");
        idIncrement = serialData.FindProperty("IDIncrement");

        if (data.FoldedOut > data.Foldouts.Count - 1)
            data.FoldedOut = -1;

        if (stageEnemies.Enemies.Count > 0)
        {
            enemyEditors = new();
            for (int i = 0; i < stageEnemies.Enemies.Count; i++)
            {
                Enemy en = stageEnemies.Enemies[i];
                enemyEditors.Add(new SingleEnemyEditor(en, enemies.GetArrayElementAtIndex(i), data, serialData));
            }
        }

        if (data.FoldedOut != -1)
        {
            selectedEnemy = (Enemy)enemies.GetArrayElementAtIndex(data.FoldedOut).managedReferenceValue;
            enemyEditors[data.FoldedOut].PrepareFoldout();
        }

        Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;
    }
    private void UndoRedo()
    {
        if (data.FoldedOut == -1 || data.TempPath.Count == 0) return;

        serialData.UpdateIfRequiredOrScript();
        serializedObject.UpdateIfRequiredOrScript();
        if (data.Foldouts.Count == 0) data.FoldedOut = -1;
        if (data.FoldedOut != -1)
            enemyEditors[data.FoldedOut].InitEditor(selectedEnemy, enemies.GetArrayElementAtIndex(data.FoldedOut), data, serialData);
        
        if (data.SelectedOption.GetPathCreator() != data.TempPath[data.SelectedPathIndex])
        {
            data.PathTypeSelection = CurrentStageEnemiesEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
            data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
        }
        else data.SelectedOption.ApplyPathOptions();

        if (data.TempPath.Count > 0)
        {
            data.TempPath[0].StartPosition = selectedEnemy.SpawnPosition;
            PathEditor.ConnectPaths(data.TempPath, 0);
        }
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data)) AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();
        Undo.undoRedoPerformed -= UndoRedo;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serialData.Update();

        EditorGUILayout.LabelField("List size: " + enemies.arraySize);
        EditorGUILayout.Space();

        if (data.IDIncrement > 0 && stageEnemies.Enemies.Count == 0)
        {
            data.FoldedOut = -1;
            data.Foldouts = new();
            data.IDIncrement = 0;
        }

        if (GUILayout.Button("New enemy"))
        {
            Enemy newEnemy = new()
            { ID = idIncrement.intValue++ };
            if (stageEnemies.Enemies.Count > 0) newEnemy.SpawnTime = stageEnemies.Enemies[^1].SpawnTime;

            enemies.arraySize++;
            enemies.GetArrayElementAtIndex(enemies.arraySize - 1).managedReferenceValue = newEnemy;
            foldouts.arraySize++;
            foldouts.GetArrayElementAtIndex(foldouts.arraySize - 1).boolValue = false;

            Undo.RecordObject(this, "Add new enemy");
            enemyEditors.Add(new SingleEnemyEditor(newEnemy, enemies.GetArrayElementAtIndex(enemies.arraySize - 1),  data, serialData));
        };

        for (int i = 0; i < enemies.arraySize; i++)
        {
            var currentEnemyEditor = enemyEditors[i];
            enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
            string label = "(" + enemy.SpawnTime.ToString("0.00") + ") " + enemy.Name + ", ID " + enemy.ID;
            foldouts.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(foldouts.GetArrayElementAtIndex(i).boolValue, label);
            if (!foldouts.GetArrayElementAtIndex(i).boolValue)
            {
                if (foldedOut.intValue == i)
                {
                    foldedOut.intValue = -1;
                    Undo.RecordObject(this, "Close foldout");
                    selectedEnemy = null;
                }
                continue;
            }
            else if (foldedOut.intValue != i)
            {
                for (int j = 0; j < foldouts.arraySize; j++) foldouts.GetArrayElementAtIndex(j).boolValue = false;
                foldouts.GetArrayElementAtIndex(i).boolValue = true;
                foldedOut.intValue = i;
                Undo.RecordObject(this, "Open foldout");
                currentEnemyEditor.InitEditor(selectedEnemy, enemies.GetArrayElementAtIndex(i), data, serialData);
                currentEnemyEditor.PrepareFoldout();

            }

            GUILayout.BeginHorizontal();
            {
                var style = new GUIStyle(GUI.skin.button);
                style.normal.textColor = new Color(0.863f, 0.078f, 0.235f);
                GUILayout.FlexibleSpace();
                bool isDelete = GUILayout.Button("Delete", style, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent("Delete")).x + 20));
                if (isDelete)
                {
                    for (int j = i; j < enemies.arraySize - 1; j++)
                    {
                        enemies.GetArrayElementAtIndex(j).managedReferenceValue = enemies.GetArrayElementAtIndex(j + 1).managedReferenceValue;
                    }
                    enemies.arraySize--;
                    for (int j = i; j < foldouts.arraySize - 1; j++)
                    {
                        foldouts.GetArrayElementAtIndex(j).boolValue = foldouts.GetArrayElementAtIndex(j + 1).boolValue;
                    }
                    foldouts.arraySize--;
                    foldedOut.intValue = -1;
                    Undo.RecordObject(this, "Close foldout");
                    enemyEditors.RemoveAt(i);
                    break;
                }
            }
            GUILayout.EndHorizontal();

            float newSpawnTime = currentEnemyEditor.DrawFoldout();
            if (newSpawnTime >= 0f)
            {
                int currId = enemies.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue;
                enemies.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnTime").floatValue = newSpawnTime;

                SortSerializedPropertyArray(enemies, 0, enemies.arraySize - 1);
                for (int j = 0; j < enemies.arraySize; j++)
                {
                    int newId = enemies.GetArrayElementAtIndex(j).FindPropertyRelative("ID").intValue;
                    if (newId == currId)
                    {
                        foldedOut.intValue = j;
                        break;
                    }
                }
                //foldedOut.intValue = Array.FindIndex(enemies, x => x.FindPropertyRelative("ID").intValue == currId);
                foldouts.GetArrayElementAtIndex(i).boolValue = false;
                foldouts.GetArrayElementAtIndex(foldedOut.intValue).boolValue = true;

                Debug.Log(currId + " " + enemies.GetArrayElementAtIndex(foldedOut.intValue).FindPropertyRelative("ID").intValue);
            }
        }

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        if (data.FoldedOut < 0) return;
        EventType e = Event.current.type;
        SingleEnemyEditor enemyEditor = enemyEditors[data.FoldedOut];
        Vector2? spawn = enemyEditor.DrawPath();
        if (spawn != null)
        {
            Undo.RecordObject(this, "Move enemy");
            enemyEditor.enemy.SpawnPosition = spawn.Value;
        }

        Repaint();
    }
}
