using EnemyClass;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using WaypointPath;

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
    //int selectedEnemyIndex = 0;
    [SerializeField]
    Enemy enemy, selectedEnemy = null;
    [SerializeField]
    bool isIncorrectPrefab = false;
    //[SerializeField]
    //GameObject enemyPrefab;
    //[SerializeField]
    //Vector2 enemySpawnPosition;
    //[SerializeField]
    //Vector2 enemyScale;
    //[SerializeField]
    //Sprite enemySprite = null;
    //List<bool> foldouts;
    //int foldedOut = -1;
    bool wasTextureMoved = false;
    //int idIncrement = 0;

    private void OnEnable()
    {
        if (stageEnemies == null) stageEnemies = target as CurrentStageEnemies;
        enemies = serializedObject.FindProperty("Enemies");
        //foldouts = new();

        data = (CurrentStageEnemiesEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(CurrentStageEnemiesEditorData));
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

        if (data.FoldedOut < data.Foldouts.Count - 1)
            data.FoldedOut = -1;

        //if (enemies.arraySize < data.FoldedOut)
        if (data.FoldedOut != -1)
        {
            selectedEnemy = (Enemy)enemies.GetArrayElementAtIndex(data.FoldedOut).managedReferenceValue;
            enemyEditors[data.FoldedOut].PrepareFoldout();
        }

        if (stageEnemies.Enemies.Count > 0)
        {
            for (int i = 0; i < stageEnemies.Enemies.Count; i++)
            {
                Enemy en = stageEnemies.Enemies[i];
                enemyEditors.Add(new SingleEnemyEditor(en, enemies.GetArrayElementAtIndex(i), data));
            }
        }

        Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;
    }
    private void UndoRedo()
    {
        Debug.Log(id.intValue);
        if (data.FoldedOut == -1 || data.TempPath.Count == 0) return;
        if (data.SelectedOption.GetPathCreator() != data.TempPath[data.SelectedPathIndex])
        {
            data.PathTypeSelection = CurrentStageEnemiesEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
            data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
            //serialData.Update();
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
            enemyEditors.Add(new SingleEnemyEditor(newEnemy, enemies.GetArrayElementAtIndex(enemies.arraySize - 1),  data));
        };

        for (int i = 0; i < enemies.arraySize; i++)
        {
            enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
            string label = "(" + enemy.SpawnTime.ToString("0.00") + ") " + enemy.Name + ", ID " + enemy.ID;
            foldouts.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(foldouts.GetArrayElementAtIndex(i).boolValue, label);
            if (!foldouts.GetArrayElementAtIndex(i).boolValue)
            {
                if (foldedOut.intValue == i)
                {
                    Undo.RecordObject(this, "Close foldout");
                    //enemySprite = null;
                    //id = spawnPosition = path = spawnRepeats = fireable = null;
                    selectedEnemy = null;
                    foldedOut.intValue = -1;
                }
                continue;
            }
            else if (foldedOut.intValue != i)
            {
                for (int j = 0; j < foldouts.arraySize; j++) foldouts.GetArrayElementAtIndex(j).boolValue = false;
                foldouts.GetArrayElementAtIndex(i).boolValue = true;
                //selectedEnemy = enemy;

                Undo.RecordObject(this, "Open foldout");
                enemyEditors[i].PrepareFoldout();
                //tempPath.arraySize = data.TempPath.Count;
                //for (int j = 0; j < tempPath.arraySize; j++)
                //    tempPath.GetArrayElementAtIndex(j).managedReferenceValue = data.TempPath[j];

                foldedOut.intValue = i;
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
                    Undo.RecordObject(this, "Close foldout");
                    enemyEditors.RemoveAt(i);
                    for (int j = i; j < foldouts.arraySize - 1; j++)
                    {
                        foldouts.GetArrayElementAtIndex(j).boolValue = foldouts.GetArrayElementAtIndex(j + 1).boolValue;
                    }
                    foldouts.arraySize--;
                    foldedOut.intValue = -1;
                    break;
                }
            }
            GUILayout.EndHorizontal();
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

        data.SelectedOption.DrawPath(enemy.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);

        Repaint();
    }
}
