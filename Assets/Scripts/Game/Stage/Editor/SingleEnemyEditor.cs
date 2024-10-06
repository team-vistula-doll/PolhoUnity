using EnemyClass;
using System;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[Serializable]
public class SingleEnemyEditor
{
    [SerializeField]
    public Enemy Enemy;
    [SerializeReference]
    public SerializedProperty SerialEnemy;
    SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path/*, spawnRepeats, fireable*/;

    [SerializeField]
    public WaypointPathEditorData Data;
    [SerializeReference]
    public SerializedObject SerialData;
    SerializedProperty prefabID, selectedPathIndex, isInsert, pathTypeSelection, tempPath;

    [SerializeField]
    GameObject prefab;
    [SerializeField]
    Sprite sprite;
    [SerializeField]
    Vector2 scale;

    bool wasTextureMoved = false;
    bool isIncorrectPrefab = false;

    public SingleEnemyEditor(Enemy enemy, SerializedProperty serialEnemy, WaypointPathEditorData data)
    {
        InitEditor(enemy, serialEnemy, data);
    }

    public void InitEditor(Enemy enemy, SerializedProperty serialEnemy, WaypointPathEditorData data)
    {
        this.Enemy = enemy;
        this.SerialEnemy = serialEnemy;
        this.Data = data;
        SerialData = new SerializedObject(this.Data);

        prefabID = SerialData.FindProperty("PrefabID");
        selectedPathIndex = SerialData.FindProperty("SelectedPathIndex");
        isInsert = SerialData.FindProperty("IsInsert");
        pathTypeSelection = SerialData.FindProperty("PathTypeSelection");
        tempPath = SerialData.FindProperty("TempPath");

        foreach (var option in WaypointPathEditorData.Options)
            option.StartPosition = enemy.SpawnPosition;

        id = serialEnemy.FindPropertyRelative("ID");
        prefabName = serialEnemy.FindPropertyRelative("PrefabName");
        enemyName = serialEnemy.FindPropertyRelative("Name");
        spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
        spawnPosition = serialEnemy.FindPropertyRelative("SpawnPosition");
        path = serialEnemy.FindPropertyRelative("Path");
        //spawnRepeats = serialEnemy.FindPropertyRelative("SpawnRepeats");
        //fireable = serialEnemy.FindPropertyRelative("Fireable");

        if (prefab == null/* && foldedOut.intValue != index*/)
        {
            //if (enemyPrefab != null) DestroyImmediate(enemyPrefab);
            GameObject newPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
                "Assets/Prefabs/Enemies/" + enemy.PrefabName + ".prefab", typeof(GameObject));
            //enemySpawnPosition.vector2Value = prefab.transform.position;
            sprite = newPrefab.GetComponent<SpriteRenderer>().sprite;
            scale = newPrefab.transform.localScale;
            prefab = newPrefab;
        }

        if (prefabID.intValue != id.intValue)
        {
            PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = path.arraySize - 2;
            if (selectedPathIndex.intValue < 0) PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = 0;
            tempPath.arraySize = 0;
            prefabID.intValue = id.intValue;
        }
    }

    public void PrepareFoldout()
    {
        SerialData.Update();
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
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = new WaypointPathExpression()
                {
                    StartPoint = new(spawnPosition.vector2Value, 0, 0)
                };
            }
            //tempPath.managedReferenceValue = new List<WaypointPathCreator>() { new WaypointPathExpression() };
            else if (path.arraySize != 0)
            {
                int tempPathOldSize = tempPath.arraySize;
                tempPath.arraySize += path.arraySize;
                for (int i = 0; i < path.arraySize; i++)
                {
                    WaypointPathCreator creator = Enemy.Path[i];
                    tempPath.GetArrayElementAtIndex(tempPathOldSize + i).managedReferenceValue = creator.GetNewAdjoinedPath(0);
                    //foreach (var creator in selectedEnemy.Path)
                    //    data.TempPath.Add(creator.GetNewAdjoinedPath(0));
                }
                WaypointPathCreator c = Enemy.Path[path.arraySize - 1];
                tempPath.arraySize++;
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = c.GetNewAdjoinedPath(1);
                //data.TempPath.Add(selectedEnemy.Path[^1].GetNewAdjoinedPath(1));
            }
        }

        pathTypeSelection.intValue = (int)WaypointPathEditorData.GetSelectedOption(
            (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
        //data.TempPath[data.SelectedPathIndex]);
        Data.SelectedOption.SetPathCreator(
            (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);

        SerialData.ApplyModifiedPropertiesWithoutUndo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>If the spawn time was changed</returns>
    public float DrawFoldout()
    {
        SerialData.Update();
        float result = -1;
        
        if (tempPath.arraySize > 1 && path.arraySize == 0)
        {
            Data.PathTypeSelection = 0;
            tempPath.ClearArray();
            WaypointPathEditorData.Options[1].SetPathCreator(new WaypointPathBezier());
            tempPath.arraySize++;
            WaypointPathCreator newExpression = new WaypointPathExpression();
            tempPath.GetArrayElementAtIndex(0).managedReferenceValue = newExpression;
            Data.SelectedOption.SetPathCreator(newExpression);
        }

        if (wasTextureMoved)
        {
            if (path.arraySize > 0)
            {
                WaypointPathCreator p = (WaypointPathCreator)path.GetArrayElementAtIndex(0).managedReferenceValue;
                p.StartPoint.Position = spawnPosition.vector2Value;
                PathEditor.ConnectPaths(path, 0);
            }
            if (tempPath.arraySize > 0)
            {
                WaypointPathCreator p = (WaypointPathCreator)tempPath.GetArrayElementAtIndex(0).managedReferenceValue;
                p.StartPoint.Position = spawnPosition.vector2Value;
                PathEditor.ConnectPaths(tempPath, 0);
            }
            Data.SelectedOption.SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            wasTextureMoved = false;
        }
        PathEditor.ConnectPaths(tempPath, 0);

        string objectPath = "Assets/Prefabs/Enemies/Enemy.prefab";
        EditorGUI.BeginChangeCheck();
        GameObject obj = (GameObject)EditorGUILayout.ObjectField(
            "Prefab", (GameObject)prefab, typeof(GameObject), false);
        if (EditorGUI.EndChangeCheck())
        {
            //Undo.RecordObject(this, "Change prefab");
            prefab = obj;
            objectPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            isIncorrectPrefab = objectPath.IndexOf("Assets/Prefabs/Enemies/") != 0;
            if (!isIncorrectPrefab)
            {
                prefabName.stringValue = objectPath.Substring(objectPath.LastIndexOf('/') + 1);
                prefabName.stringValue = prefabName.stringValue.Substring(0, prefabName.stringValue.LastIndexOf('.'));
                //spawnPosition.vector2Value = obj.transform.position;
                scale = obj.transform.localScale;
                sprite = obj.GetComponent<SpriteRenderer>().sprite;
            }
        }
        if (isIncorrectPrefab)
            EditorGUILayout.HelpBox("The provided object isn't in the enemies folder!", MessageType.Warning);

        EditorGUI.BeginChangeCheck();
        float newSpawnTime = EditorGUILayout.DelayedFloatField("Spawn time", spawnTime.floatValue);
        if (spawnTime.floatValue < 0) spawnTime.floatValue = 0;
        if (EditorGUI.EndChangeCheck())
        {
            result = newSpawnTime;
        }
        EditorGUILayout.PropertyField(enemyName);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(spawnPosition);
        if (EditorGUI.EndChangeCheck())
            wasTextureMoved = true;

        Data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, Enemy.Path);

        selectedPathIndex.intValue -= Data.SelectedOption.DeletePath(path, tempPath);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(pathTypeSelection, selectedPathIndex, tempPath);

        if (Data.SelectedOption.PathOptions()) PathEditor.ConnectPaths(Data.TempPath, selectedPathIndex.intValue);

        if (Data.SelectedOption.SetPath(path, isInsert, selectedPathIndex, tempPath))
        {
            pathTypeSelection.intValue = (int)WaypointPathEditorData.GetSelectedOption(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            WaypointPathEditorData.Options[pathTypeSelection.intValue].SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            //EditorUtility.SetDirty(stageEnemies);
        }

        EditorGUILayout.Space();
        SerialData.ApplyModifiedProperties();
        return result;
    }

    public Vector2? DrawPath()
    {
        Vector2? result = null;

        EventType e = Event.current.type;
        Vector2 screenPosition = HandleUtility.WorldToGUIPoint(spawnPosition.vector2Value);
        Vector2 screenScale = sprite.rect.size / HandleUtility.GetHandleSize(spawnPosition.vector2Value) * scale;
        Handles.BeginGUI();
        GUI.DrawTexture(new Rect(
            screenPosition - screenScale / 2, screenScale),
            sprite.texture);
        Handles.EndGUI();

        EditorGUI.BeginChangeCheck();
        Vector2 newSpawnPosition = Handles.PositionHandle(spawnPosition.vector2Value, Quaternion.identity);
        if (EditorGUI.EndChangeCheck())
        {
            //Undo.RecordObject(enemy, "")
            //WaypointPathCreator creator = (WaypointPathCreator)tempPath.GetArrayElementAtIndex(0).managedReferenceValue;
            //creator.StartPosition = newSpawnPosition;
            //tempPath.GetArrayElementAtIndex(0).managedReferenceValue = creator;
            //PathEditor.ConnectPaths(tempPath, 0);
            wasTextureMoved = true;
            result = newSpawnPosition;
        }

        Data.SelectedOption.DrawPath(Enemy.Path, 0, e, false);
        Data.SelectedOption.DrawPath(Data.TempPath, selectedPathIndex.intValue, e, true);
        return result;
    }
}
