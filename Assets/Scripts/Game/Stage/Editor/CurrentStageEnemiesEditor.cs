using EnemyClass;
using System;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    CurrentStageEnemies stageEnemies;
    SerializedProperty enemies;
    SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path, spawnRepeats, fireable;

    CurrentStageEnemiesEditorData data;
    SerializedObject serialData;
    SerializedProperty prefabID, selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    SerializedProperty enemyPrefab, enemyScale, enemySprite, foldouts, foldedOut, idIncrement;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";

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

        enemyPrefab = serialData.FindProperty("EnemyPrefab");
        //enemySpawnPosition = serialData.FindProperty("EnemySpawnPosition");
        enemyScale = serialData.FindProperty("EnemyScale");
        enemySprite = serialData.FindProperty("EnemySprite");
        foldouts = serialData.FindProperty("Foldouts");
        foldedOut = serialData.FindProperty("FoldedOut");
        idIncrement = serialData.FindProperty("IDIncrement");

        if (data.FoldedOut != -1)
        {
            enemy = (Enemy)enemies.GetArrayElementAtIndex(data.FoldedOut).managedReferenceValue;
            PrepareFoldout(data.FoldedOut);
        }
        Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;
    }
    private void UndoRedo()
    {
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

        if (data.IDIncrement > 0 && stageEnemies.Enemies.Count == 0) data.IDIncrement = 0; 

        if (GUILayout.Button("New enemy"))
        {
            Enemy newEnemy = new()
            { ID = idIncrement.intValue++ };
            if (stageEnemies.Enemies.Count > 0) newEnemy.SpawnTime = stageEnemies.Enemies[^1].SpawnTime;
            enemies.arraySize++;
            enemies.GetArrayElementAtIndex(enemies.arraySize - 1).managedReferenceValue = newEnemy;
            foldouts.arraySize++;
            foldouts.GetArrayElementAtIndex(foldouts.arraySize - 1).boolValue = false;
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
                    enemySprite = null;
                    //id = spawnPosition = path = spawnRepeats = fireable = null;
                    selectedEnemy = null;
                    foldedOut.intValue = -1;
                }
                continue;
            }
            else if (foldedOut.intValue != i)
            {
                PrepareFoldout(i);
                //tempPath.arraySize = data.TempPath.Count;
                //for (int j = 0; j < tempPath.arraySize; j++)
                //    tempPath.GetArrayElementAtIndex(j).managedReferenceValue = data.TempPath[j];

                foldedOut.intValue = i;
            }

            if (tempPath.arraySize > 1 && selectedEnemy.Path.Count == 0)
            {
                data.PathTypeSelection = 0;
                serialData.Update();
                tempPath.ClearArray();
                CurrentStageEnemiesEditorData.Options[1].SetPathCreator(new WaypointPathBezier());
                tempPath.arraySize++;
                WaypointPathCreator newExpression = new WaypointPathExpression();
                tempPath.GetArrayElementAtIndex(0).managedReferenceValue = newExpression;
                data.SelectedOption.SetPathCreator(newExpression);
            }

            if (wasTextureMoved)
            {
                if (path.arraySize > 0)
                {
                    WaypointPathCreator p = (WaypointPathCreator)path.GetArrayElementAtIndex(0).managedReferenceValue;
                    p.StartPosition = spawnPosition.vector2Value;
                    path.GetArrayElementAtIndex(0).managedReferenceValue = p;
                    PathEditor.ConnectPaths(path, 0);
                }
                if (tempPath.arraySize > 0)
                {
                    WaypointPathCreator p = (WaypointPathCreator)tempPath.GetArrayElementAtIndex(0).managedReferenceValue;
                    p.StartPosition = spawnPosition.vector2Value;
                    tempPath.GetArrayElementAtIndex(0).managedReferenceValue = p;
                    PathEditor.ConnectPaths(tempPath, 0);
                }
                data.SelectedOption.SetPathCreator(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                wasTextureMoved = false;
            }
            PathEditor.ConnectPaths(tempPath, 0);

            string objectPath = "Assets/Prefabs/Enemies/Enemy.prefab";
            EditorGUI.BeginChangeCheck();
            GameObject obj = (GameObject)EditorGUILayout.ObjectField(
                "Prefab", (GameObject)enemyPrefab.objectReferenceValue, typeof(GameObject), false);
            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject(this, "Change prefab");
                enemyPrefab.objectReferenceValue = obj;
                objectPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                isIncorrectPrefab = objectPath.IndexOf("Assets/Prefabs/Enemies/") != 0;
                if (!isIncorrectPrefab)
                {
                    prefabName.stringValue = objectPath.Substring(objectPath.LastIndexOf('/') + 1);
                    prefabName.stringValue = prefabName.stringValue.Substring(0, prefabName.stringValue.LastIndexOf('.') );
                    //spawnPosition.vector2Value = obj.transform.position;
                    enemyScale.vector2Value = obj.transform.localScale;
                    enemySprite.objectReferenceValue = obj.GetComponent<SpriteRenderer>().sprite;
                }
            }
            if (isIncorrectPrefab)
                EditorGUILayout.HelpBox("The provided object isn't in the enemies folder!", MessageType.Warning);

            EditorGUI.BeginChangeCheck();
            spawnTime.floatValue = EditorGUILayout.DelayedFloatField("Spawn time", spawnTime.floatValue);
            if (spawnTime.floatValue < 0) spawnTime.floatValue = 0;
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                int currId = id.intValue;
                Enemy[] list = new Enemy[enemies.arraySize];
                for (int j = 0; j < enemies.arraySize; j++) list[j] = (Enemy)enemies.GetArrayElementAtIndex(j).managedReferenceValue;
                //list[i].SpawnTime = spawnTime.floatValue;
                Array.Sort(list);
                for (int j = 0; j < enemies.arraySize; j++) enemies.GetArrayElementAtIndex(j).managedReferenceValue = list[j];
                foldedOut.intValue = Array.FindIndex(list, x => x.ID == currId);
                foldouts.GetArrayElementAtIndex(foldedOut.intValue).boolValue = true;
                foldouts.GetArrayElementAtIndex(i).boolValue = false;
            }
            EditorGUILayout.PropertyField(enemyName);
            EditorGUILayout.PropertyField(spawnPosition);

            data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, selectedEnemy.Path);

            selectedPathIndex.intValue -= data.SelectedOption.DeletePath(path, tempPath);
            if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

            EditorGUILayout.Space();
            PathEditor.PathTypes(pathTypeSelection, selectedPathIndex, tempPath);

            if (data.SelectedOption.PathOptions()) PathEditor.ConnectPaths(data.TempPath, selectedPathIndex.intValue);

            if (data.SelectedOption.SetPath(path, isInsert, selectedPathIndex, tempPath))
            {
                pathTypeSelection.intValue = (int)CurrentStageEnemiesEditorData.GetSelectedOption(
                        (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                CurrentStageEnemiesEditorData.Options[pathTypeSelection.intValue].SetPathCreator(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                EditorUtility.SetDirty(stageEnemies);
            }
        }

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    private void PrepareFoldout(int index)
    {
        if (index < 0) throw new ArgumentException("Negative foldout number to be painted");

        Undo.RecordObject(this, "Open foldout");
        for (int j = 0; j < foldouts.arraySize; j++) foldouts.GetArrayElementAtIndex(j).boolValue = false;
        foldouts.GetArrayElementAtIndex(index).boolValue = true;
        selectedEnemy = enemy;
        foreach (var option in CurrentStageEnemiesEditorData.Options) option.StartPosition = selectedEnemy.SpawnPosition;

        SerializedProperty serialEnemy = enemies.GetArrayElementAtIndex(index);
        id = serialEnemy.FindPropertyRelative("ID");
        prefabName = serialEnemy.FindPropertyRelative("PrefabName");
        enemyName = serialEnemy.FindPropertyRelative("Name");
        spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
        spawnPosition = serialEnemy.FindPropertyRelative("SpawnPosition");
        path = serialEnemy.FindPropertyRelative("Path");
        spawnRepeats = serialEnemy.FindPropertyRelative("SpawnRepeats");
        fireable = serialEnemy.FindPropertyRelative("Fireable");

        if (enemyPrefab.objectReferenceValue == null && foldedOut.intValue != index)
        {
            //if (enemyPrefab != null) DestroyImmediate(enemyPrefab);
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(
                "Assets/Prefabs/Enemies/" + selectedEnemy.PrefabName + ".prefab", typeof(GameObject));
            //enemySpawnPosition.vector2Value = prefab.transform.position;
            enemySprite.objectReferenceValue = prefab.GetComponent<SpriteRenderer>().sprite;
            enemyScale.vector2Value = prefab.transform.localScale;
            enemyPrefab.objectReferenceValue = prefab;
        }

        if (prefabID.intValue != id.intValue)
        {
            PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = path.arraySize - 2;
            if (selectedPathIndex.intValue < 0) PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = 0;
            tempPath.arraySize = 0;
            prefabID.intValue = id.intValue;
        }

        if (tempPath.arraySize != 0)
        {
            int insert = isInsert.boolValue ? 2 : 1;
            if (tempPath.arraySize > path.arraySize + insert)
            {
                for (int i = path.arraySize; i < tempPath.arraySize - insert; i++)
                    tempPath.GetArrayElementAtIndex(i).managedReferenceValue = tempPath.GetArrayElementAtIndex(
                        i + tempPath.arraySize - (path.arraySize + insert)).managedReferenceValue;
                tempPath.arraySize -= insert;
                //data.TempPath.RemoveRange(selectedEnemy.Path.Count, data.TempPath.Count - (selectedEnemy.Path.Count + insert));
            }
        }
        else
        {
            if (path.arraySize == 0)
            {
                tempPath.arraySize = 1;
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = new WaypointPathExpression();
            }
                //tempPath.managedReferenceValue = new List<WaypointPathCreator>() { new WaypointPathExpression() };
            else if (path.arraySize != 0)
            {
                int tempPathOldSize = tempPath.arraySize;
                tempPath.arraySize += path.arraySize;
                for (int i = 0; i < path.arraySize; i++)
                {
                    WaypointPathCreator creator = (WaypointPathCreator)path.GetArrayElementAtIndex(i).managedReferenceValue;
                    tempPath.GetArrayElementAtIndex(tempPathOldSize + i).managedReferenceValue = creator.GetNewAdjoinedPath(0);
                //foreach (var creator in selectedEnemy.Path)
                //    data.TempPath.Add(creator.GetNewAdjoinedPath(0));
                }
                WaypointPathCreator c = (WaypointPathCreator)path.GetArrayElementAtIndex(path.arraySize - 1).managedReferenceValue;
                tempPath.arraySize++;
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = c.GetNewAdjoinedPath(1);
                //data.TempPath.Add(selectedEnemy.Path[^1].GetNewAdjoinedPath(1));
            }
        }

        pathTypeSelection.intValue = (int)CurrentStageEnemiesEditorData.GetSelectedOption(
            (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            //data.TempPath[data.SelectedPathIndex]);
        data.SelectedOption.SetPathCreator(
            (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);

    }

    public void OnSceneGUI()
    {
        if (data.FoldedOut == -1 || selectedEnemy == null) return;
        EventType e = Event.current.type;

        Vector2 screenPosition = HandleUtility.WorldToGUIPoint(spawnPosition.vector2Value);
        Vector2 screenScale = data.EnemySprite.rect.size / HandleUtility.GetHandleSize(spawnPosition.vector2Value) * data.EnemyScale;
        Handles.BeginGUI();
        GUI.DrawTexture(new Rect(
            screenPosition - screenScale / 2, screenScale),
            data.EnemySprite.texture);
        Handles.EndGUI();

        EditorGUI.BeginChangeCheck();
        Vector2 newSpawnPosition = Handles.PositionHandle(spawnPosition.vector2Value, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(this, "Moved enemy");
            selectedEnemy.SpawnPosition = spawnPosition.vector2Value = newSpawnPosition;
            wasTextureMoved = true;
            //Repaint();
        }

        data.SelectedOption.DrawPath(selectedEnemy.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);
        Repaint();
    }
}
