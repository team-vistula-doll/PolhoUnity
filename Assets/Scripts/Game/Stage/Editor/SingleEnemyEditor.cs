using EnemyClass;
using System;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[Serializable]
public class SingleEnemyEditor
{
    public Enemy enemy;
    SerializedProperty serialEnemy;
    SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path/*, spawnRepeats, fireable*/;

    CurrentStageEnemiesEditorData data;
    public SerializedObject serialData;
    SerializedProperty prefabID, selectedPathIndex, isInsert, pathTypeSelection, tempPath;

    GameObject prefab;
    Sprite sprite;
    Vector2 scale;

    bool wasTextureMoved = false;
    bool isIncorrectPrefab = false;

    public SingleEnemyEditor(Enemy enemy, SerializedProperty serialEnemy, CurrentStageEnemiesEditorData data, SerializedObject serialData)
    {
        this.enemy = enemy;
        this.serialEnemy = serialEnemy;
        this.data = data;

        this.serialData = serialData;
        prefabID = serialData.FindProperty("PrefabID");
        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");

        foreach (var option in CurrentStageEnemiesEditorData.Options) option.StartPosition = enemy.SpawnPosition;

        id = serialEnemy.FindPropertyRelative("ID");
        prefabName = serialEnemy.FindPropertyRelative("PrefabName");
        enemyName = serialEnemy.FindPropertyRelative("Name");
        spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
        spawnPosition = serialEnemy.FindPropertyRelative("SpawnPosition");
        path = serialEnemy.FindPropertyRelative("Path");
        //spawnRepeats = serialEnemy.FindPropertyRelative("SpawnRepeats");
        //fireable = serialEnemy.FindPropertyRelative("Fireable");
        Debug.Log(enemy.ID);
    }

    public void PrepareFoldout()
    {

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

        if (data.PrefabID != enemy.ID)
        {
            PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = enemy.Path.Count - 2;
            if (selectedPathIndex.intValue < 0) PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = selectedPathIndex.intValue = 0;
            tempPath.arraySize = 0;
            prefabID.intValue = enemy.ID;
        }

        if (tempPath.arraySize != 0)
        {
            int insert = isInsert.boolValue ? 2 : 1;
            if (tempPath.arraySize > enemy.Path.Count + insert)
            {
                for (int i = enemy.Path.Count; i < tempPath.arraySize - insert; i++)
                    tempPath.GetArrayElementAtIndex(i).managedReferenceValue = tempPath.GetArrayElementAtIndex(
                        i + tempPath.arraySize - (enemy.Path.Count + insert)).managedReferenceValue;
                tempPath.arraySize -= insert;
                //data.TempPath.RemoveRange(selectedEnemy.Path.Count, data.TempPath.Count - (selectedEnemy.Path.Count + insert));
            }
        }
        else
        {
            if (enemy.Path.Count == 0)
            {
                tempPath.arraySize = 1;
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = new WaypointPathExpression()
                {
                    StartPosition = spawnPosition.vector2Value
                };
            }
            //tempPath.managedReferenceValue = new List<WaypointPathCreator>() { new WaypointPathExpression() };
            else if (enemy.Path.Count != 0)
            {
                int tempPathOldSize = tempPath.arraySize;
                tempPath.arraySize += enemy.Path.Count;
                for (int i = 0; i < enemy.Path.Count; i++)
                {
                    WaypointPathCreator creator = enemy.Path[i];
                    tempPath.GetArrayElementAtIndex(tempPathOldSize + i).managedReferenceValue = creator.GetNewAdjoinedPath(0);
                    //foreach (var creator in selectedEnemy.Path)
                    //    data.TempPath.Add(creator.GetNewAdjoinedPath(0));
                }
                WaypointPathCreator c = enemy.Path[enemy.Path.Count - 1];
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

        serialData.ApplyModifiedPropertiesWithoutUndo();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>If the spawn time was changed</returns>
    public bool DrawFoldout()
    {
        bool result = false;
        //enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
        //string label = "(" + enemy.SpawnTime.ToString("0.00") + ") " + enemy.Name + ", ID " + enemy.ID;
        //foldouts.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(foldouts.GetArrayElementAtIndex(i).boolValue, label);
        //if (!foldouts.GetArrayElementAtIndex(i).boolValue)
        //{
        //    if (foldedOut.intValue == i)
        //    {
        //        Undo.RecordObject(this, "Close foldout");
        //        enemySprite = null;
        //        //id = spawnPosition = path = spawnRepeats = fireable = null;
        //        enemy = null;
        //        foldedOut.intValue = -1;
        //    }
        //    continue;
        //}
        //else if (foldedOut.intValue != i)
        //{
        //    PrepareFoldout(i);
        //    //tempPath.arraySize = data.TempPath.Count;
        //    //for (int j = 0; j < tempPath.arraySize; j++)
        //    //    tempPath.GetArrayElementAtIndex(j).managedReferenceValue = data.TempPath[j];

        //    foldedOut.intValue = i;
        //}

        //GUILayout.BeginHorizontal();
        //{
        //    var style = new GUIStyle(GUI.skin.button);
        //    style.normal.textColor = new Color(0.863f, 0.078f, 0.235f);
        //    GUILayout.FlexibleSpace();
        //    bool isDelete = GUILayout.Button("Delete", style, GUILayout.MaxWidth(EditorStyles.label.CalcSize(new GUIContent("Delete")).x + 20));
        //    if (isDelete)
        //    {
        //        for (int j = i; j < enemies.arraySize - 1; j++)
        //        {
        //            enemies.GetArrayElementAtIndex(j).managedReferenceValue = enemies.GetArrayElementAtIndex(j + 1).managedReferenceValue;
        //        }
        //        enemies.arraySize--;

        //        for (int j = i; j < foldouts.arraySize - 1; j++)
        //        {
        //            foldouts.GetArrayElementAtIndex(j).boolValue = foldouts.GetArrayElementAtIndex(j + 1).boolValue;
        //        }
        //        foldouts.arraySize--;
        //        foldedOut.intValue = -1;
        //        break;
        //    }
        //}
        //GUILayout.EndHorizontal();

        if (tempPath.arraySize > 1 && path.arraySize == 0)
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
        spawnTime.floatValue = EditorGUILayout.DelayedFloatField("Spawn time", spawnTime.floatValue);
        if (spawnTime.floatValue < 0) spawnTime.floatValue = 0;
        if (EditorGUI.EndChangeCheck())
        {
            result = true;
        }
        EditorGUILayout.PropertyField(enemyName);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(spawnPosition);
        if (EditorGUI.EndChangeCheck())
            wasTextureMoved = true;

        data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, enemy.Path);

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
            //EditorUtility.SetDirty(stageEnemies);
        }

        serialData.ApplyModifiedPropertiesWithoutUndo();
        return result;
    }

    public Vector2? DrawPath()
    {
        Vector2? result = null;

        EventType e = Event.current.type;
        Vector2 screenPosition = HandleUtility.WorldToGUIPoint(enemy.SpawnPosition);
        Vector2 screenScale = sprite.rect.size / HandleUtility.GetHandleSize(enemy.SpawnPosition) * scale;
        Handles.BeginGUI();
        GUI.DrawTexture(new Rect(
            screenPosition - screenScale / 2, screenScale),
            sprite.texture);
        Handles.EndGUI();

        EditorGUI.BeginChangeCheck();
        Vector2 newSpawnPosition = Handles.PositionHandle(enemy.SpawnPosition, Quaternion.identity);
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

        data.SelectedOption.DrawPath(enemy.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);
        return result;
    }
}
