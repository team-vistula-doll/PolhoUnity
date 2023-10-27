using EnemyClass;
using System;
using System.Collections.Generic;
using System.Linq;
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
    //SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path/*, spawnRepeats, fireable*/;

    CurrentStageEnemiesEditorData data;
    SerializedObject serialData;
    SerializedProperty foldouts, foldedOut, idIncrement;

    [SerializeReference]
    List<WaypointPathEditorData> enemyDatas = new();
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
        else if (enemyDatas == null || enemyDatas.Count == 0)
        {
            WaypointPathEditorData[] loadedAssetArray = Array.ConvertAll(AssetDatabase.LoadAllAssetsAtPath(assetPath), item => (WaypointPathEditorData)item);
            enemyDatas.AddRange(Array.ConvertAll(AssetDatabase.LoadAllAssetsAtPath(assetPath), item => (WaypointPathEditorData)item));
        }

        //data.Init();
        serialData = new SerializedObject(data);
        for (int i = 0; i < enemies.arraySize; i++) data.Foldouts.Add(false);
        if (stageEnemies.Enemies.Count > 0) data.IDIncrement = stageEnemies.Enemies[^1].ID + 1;

        foldouts = serialData.FindProperty("Foldouts");
        foldedOut = serialData.FindProperty("FoldedOut");
        idIncrement = serialData.FindProperty("IDIncrement");

        if (data.FoldedOut > data.Foldouts.Count - 1)
            data.FoldedOut = -1;

        WaypointPathEditorData.Init();
        if (stageEnemies.Enemies.Count > 0)
        {
            enemyEditors = new();
            for (int i = 0; i < stageEnemies.Enemies.Count; i++)
            {
                Enemy en = stageEnemies.Enemies[i];
                WaypointPathEditorData enemyData;
                if (enemyDatas == null || enemyDatas.Count < i)
                {
                    enemyData = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
                    enemyDatas.Add(enemyData);
                }
                else enemyData = enemyDatas[i];
                enemyEditors.Add(new SingleEnemyEditor(en, enemies.GetArrayElementAtIndex(i), enemyData));
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
        if (data.FoldedOut == -1 || enemyDatas[data.FoldedOut].TempPath.Count == 0) return;

        WaypointPathEditorData currentEnemyData = enemyDatas[data.FoldedOut];
        serialData.UpdateIfRequiredOrScript();
        serializedObject.UpdateIfRequiredOrScript();
        if (data.Foldouts.Count == 0) data.FoldedOut = -1;
        if (data.FoldedOut != -1)
        {
            WaypointPathEditorData enemyData;
            if (enemyDatas == null || enemyDatas.Count < data.FoldedOut)
            {
                enemyData = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
                enemyDatas.Add(enemyData);
            }
            else enemyData = enemyDatas[data.FoldedOut];
            enemyEditors[data.FoldedOut].InitEditor(selectedEnemy, enemies.GetArrayElementAtIndex(data.FoldedOut), enemyData);
        }
        
        if (currentEnemyData.SelectedOption.GetPathCreator() != currentEnemyData.TempPath[currentEnemyData.SelectedPathIndex])
        {
            currentEnemyData.PathTypeSelection = WaypointPathEditorData.GetSelectedOption(currentEnemyData.TempPath[currentEnemyData.SelectedPathIndex]);
            currentEnemyData.SelectedOption.SetPathCreator(currentEnemyData.TempPath[currentEnemyData.SelectedPathIndex]);
        }
        else currentEnemyData.SelectedOption.ApplyPathOptions();

        if (currentEnemyData.TempPath.Count > 0)
        {
            currentEnemyData.TempPath[0].StartPosition = selectedEnemy.SpawnPosition;
            PathEditor.ConnectPaths(currentEnemyData.TempPath, 0);
        }
    }

    private void OnDisable()
    {
        if (!AssetDatabase.Contains(data))
        {
            AssetDatabase.CreateAsset(data, assetPath);
            foreach (var ed in enemyDatas) AssetDatabase.AddObjectToAsset(ed, assetPath);
        }
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
            enemyDatas.Add((WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData)));
            enemyEditors.Add(new SingleEnemyEditor(newEnemy, enemies.GetArrayElementAtIndex(enemies.arraySize - 1),
                enemyDatas[foldouts.arraySize - 1]));
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
                currentEnemyEditor.InitEditor(selectedEnemy, enemies.GetArrayElementAtIndex(i), enemyDatas[i]);
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
                    enemyDatas.RemoveAt(i);
                    break;
                }
            }
            GUILayout.EndHorizontal();

            float newSpawnTime = currentEnemyEditor.DrawFoldout();
            if (newSpawnTime >= 0f)
            {
                int currId = enemies.GetArrayElementAtIndex(i).FindPropertyRelative("ID").intValue;
                enemies.GetArrayElementAtIndex(i).FindPropertyRelative("SpawnTime").floatValue = newSpawnTime;

                IComparable[] keys = new IComparable[enemies.arraySize];
                for (int j = 0; j < enemies.arraySize; j++)
                    keys[j] = enemies.GetArrayElementAtIndex(j).FindPropertyRelative("SpawnTime").floatValue;

                SortSerializedPropertyArray(enemies, keys, 0, enemies.arraySize - 1);

                for (int j = 0; j < enemies.arraySize; j++)
                {
                    Debug.Log("Element " + j + " spawnTime = " +
                        enemies.GetArrayElementAtIndex(j).FindPropertyRelative("SpawnTime").floatValue);
                    int newId = enemies.GetArrayElementAtIndex(j).FindPropertyRelative("ID").intValue;
                    if (newId == currId)
                    {
                        foldedOut.intValue = j;
                        break;
                    }
                }
                Debug.Log(enemies.GetArrayElementAtIndex(foldedOut.intValue)
                    .FindPropertyRelative("SpawnTime").floatValue);
                //foldedOut.intValue = Array.FindIndex(enemies, x => x.FindPropertyRelative("ID").intValue == currId);
                foldouts.GetArrayElementAtIndex(i).boolValue = false;
                foldouts.GetArrayElementAtIndex(foldedOut.intValue).boolValue = true;

                //Debug.Log(currId + " " + enemies.GetArrayElementAtIndex(foldedOut.intValue).FindPropertyRelative("ID").intValue);
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
