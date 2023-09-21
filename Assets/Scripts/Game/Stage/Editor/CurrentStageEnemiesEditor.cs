using EnemyClass;
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

    //int selectedEnemyIndex = 0;
    SerializedProperty serialEnemy;
    SerializedProperty id, enemyName, spawnTime, spawnPosition, path, spawnRepeats, fireable;
    Enemy enemy, selectedEnemy = null;
    [SerializeField]
    Vector2 enemySpawnPosition;
    Vector2 enemyScale;
    Sprite enemySprite = null;
    List<bool> foldouts;
    int foldedOut = -1;
    bool wasTextureMoved = false;
    int idIncrement = 0;

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
    private void UndoRedo()
    {
        if (data.SelectedOption.GetPathCreator() != data.TempPath[data.SelectedPathIndex])
        {
            data.PathTypeSelection = WaypointPathEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
            data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
            //serialData.Update();
        }
        else data.SelectedOption.ApplyPathOptions();
        PathEditor.ConnectPaths(data.TempPath, 0);
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
            string label = "(" + enemy.SpawnTime.ToString("0.00") + ") Enemy " + (i+1);
            foldouts[i] = EditorGUILayout.Foldout(foldouts[i], label);
            if (!foldouts[i])
            {
                if (foldedOut == i)
                {
                    enemySprite = null;
                    id = spawnPosition = path = spawnRepeats = fireable = null;
                    selectedEnemy = null;
                    foldedOut = -1;
                }
                continue;
            }
            else if (foldedOut != i)
            {
                for (int j = 0; j < foldouts.Count; j++) foldouts[j] = false;
                foldouts[i] = true;
                selectedEnemy = enemy;
                selectedEnemy.ID = idIncrement++;
                foreach (var option in WaypointPathEditorData.Options) option.StartPosition = selectedEnemy.SpawnPosition;

                serialEnemy = enemies.GetArrayElementAtIndex(i);
                id = serialEnemy.FindPropertyRelative("ID");
                enemyName = serialEnemy.FindPropertyRelative("Name");
                spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
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

                int newId = selectedEnemy.ID;
                if (data.ID != newId)
                {
                    PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = data.SelectedPathIndex = 0;
                    data.TempPath.Clear();
                    data.ID = newId;
                }

                if (data.TempPath != null && data.TempPath.Count != 0)
                {
                    int insert = data.IsInsert ? 2 : 1;
                    if (data.TempPath.Count > selectedEnemy.Path.Count + insert)
                        data.TempPath.RemoveRange(selectedEnemy.Path.Count, data.TempPath.Count - (selectedEnemy.Path.Count + insert));
                }
                else
                {
                    if (selectedEnemy.Path == null || selectedEnemy.Path.Count == 0)
                        data.TempPath = new() { new WaypointPathExpression() };
                    else if (selectedEnemy.Path != null && selectedEnemy.Path.Count != 0)
                    {
                        foreach (var creator in selectedEnemy.Path)
                            data.TempPath.Add(creator.GetNewAdjoinedPath(0));
                        data.TempPath.Add(selectedEnemy.Path[^1].GetNewAdjoinedPath(1));
                    }
                }

                data.PathTypeSelection = WaypointPathEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
                data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
                Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;

                foldedOut = i;
            }

            if (tempPath.arraySize > 1 && selectedEnemy.Path.Count == 0)
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

            if (wasTextureMoved)
            {
                data.SelectedOption.StartPosition = selectedEnemy.SpawnPosition;
                PathEditor.ConnectPaths(selectedEnemy.Path, 0);
                serializedObject.UpdateIfRequiredOrScript();
                PathEditor.ConnectPaths(tempPath, 0);
                data.SelectedOption.SetPathCreator(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                wasTextureMoved = false;
            }
            PathEditor.ConnectPaths(tempPath, 0);

            enemySpawnPosition = EditorGUILayout.Vector2Field("Spawn Position", enemySpawnPosition);

            data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, selectedEnemy.Path);

            selectedPathIndex.intValue -= data.SelectedOption.DeletePath(path, tempPath);
            if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

            EditorGUILayout.Space();
            PathEditor.PathTypes(pathTypeSelection, selectedPathIndex, tempPath);

            if (data.SelectedOption.PathOptions()) PathEditor.ConnectPaths(data.TempPath, selectedPathIndex.intValue);

            if (data.SelectedOption.SetPath(path, isInsert, selectedPathIndex, tempPath))
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
        if (foldedOut == -1 || selectedEnemy == null) return;
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
            selectedEnemy.SpawnPosition = enemySpawnPosition = newSpawnPosition;
            wasTextureMoved = true;
            //Repaint();
        }

        data.SelectedOption.DrawPath(selectedEnemy.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);
        Repaint();

        //PathEditor.DrawPath(false, e);
    }
}
