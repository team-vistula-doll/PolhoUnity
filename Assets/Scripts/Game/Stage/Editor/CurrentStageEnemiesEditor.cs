using EnemyClass;
using UnityEditor;
using UnityEngine;
using WaypointPath;

[CustomEditor(typeof(CurrentStageEnemies))]
[CanEditMultipleObjects]
public class CurrentStageEnemiesEditor : Editor
{
    CurrentStageEnemies stageEnemies;
    SerializedProperty enemies;

    CurrentStageEnemiesEditorData data;

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    const string assetPath = "Assets/Editor Assets/CurrentStageEnemiesEditorData.asset";

    //int selectedEnemyIndex = 0;
    SerializedProperty serialEnemy;
    SerializedProperty id, prefabName, enemyName, spawnTime, spawnPosition, path, spawnRepeats, fireable;
    SerializedProperty foldouts, foldedOut;
    Enemy enemy, selectedEnemy = null;
    [SerializeField]
    Vector2 enemySpawnPosition;
    [SerializeField]
    GameObject enemyPrefab;
    [SerializeField]
    bool isIncorrectPrefab = false;
    [SerializeField]
    Vector2 enemyScale;
    [SerializeField]
    Sprite enemySprite = null;
    //List<bool> foldouts;
    //int foldedOut = -1;
    bool wasTextureMoved = false;
    int idIncrement = 0;

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

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");

        foldouts = serialData.FindProperty("Foldouts");
        foldedOut = serialData.FindProperty("FoldedOut");
    }
    private void UndoRedo()
    {
        if (data.SelectedOption.GetPathCreator() != data.TempPath[data.SelectedPathIndex])
        {
            data.PathTypeSelection = CurrentStageEnemiesEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
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
            foldouts.arraySize++;
            foldouts.GetArrayElementAtIndex(foldouts.arraySize - 1).boolValue = false;
        };

        for (int i = 0; i < enemies.arraySize; i++)
        {
            enemy = (Enemy)enemies.GetArrayElementAtIndex(i).managedReferenceValue;
            string label = "(" + enemy.SpawnTime.ToString("0.00") + ") " + enemy.Name + " " + (i+1);
            foldouts.GetArrayElementAtIndex(i).boolValue = EditorGUILayout.Foldout(foldouts.GetArrayElementAtIndex(i).boolValue, label);
            if (!foldouts.GetArrayElementAtIndex(i).boolValue)
            {
                if (foldedOut.intValue == i)
                {
                    enemySprite = null;
                    id = spawnPosition = path = spawnRepeats = fireable = null;
                    selectedEnemy = null;
                    foldedOut.intValue = -1;
                }
                continue;
            }
            else if (foldedOut.intValue != i)
            {
                for (int j = 0; j < foldouts.arraySize; j++) foldouts.GetArrayElementAtIndex(j).boolValue = false;
                foldouts.GetArrayElementAtIndex(i).boolValue = true;
                selectedEnemy = enemy;
                selectedEnemy.ID = idIncrement++;
                foreach (var option in CurrentStageEnemiesEditorData.Options) option.StartPosition = selectedEnemy.SpawnPosition;

                serialEnemy = enemies.GetArrayElementAtIndex(i);
                id = serialEnemy.FindPropertyRelative("ID");
                prefabName = serialEnemy.FindPropertyRelative("PrefabName");
                enemyName = serialEnemy.FindPropertyRelative("Name");
                spawnTime = serialEnemy.FindPropertyRelative("SpawnTime");
                spawnPosition = serialEnemy.FindPropertyRelative("SpawnPosition");
                path = serialEnemy.FindPropertyRelative("Path");
                spawnRepeats = serialEnemy.FindPropertyRelative("SpawnRepeats");
                fireable = serialEnemy.FindPropertyRelative("Fireable");

                if (enemyPrefab == null && foldedOut.intValue != i)
                {
                    //if (enemyPrefab != null) DestroyImmediate(enemyPrefab);
                    enemyPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
                        "Assets/Prefabs/Enemies/" + selectedEnemy.PrefabName + ".prefab", typeof(GameObject));
                    enemySpawnPosition = enemyPrefab.transform.position;
                    enemySprite = enemyPrefab.GetComponent<SpriteRenderer>().sprite;
                    enemyScale = enemyPrefab.transform.localScale;
                }

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

                data.PathTypeSelection = CurrentStageEnemiesEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
                data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
                Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;

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
                data.SelectedOption.StartPosition = selectedEnemy.SpawnPosition;
                PathEditor.ConnectPaths(selectedEnemy.Path, 0);
                serializedObject.UpdateIfRequiredOrScript();
                PathEditor.ConnectPaths(tempPath, 0);
                data.SelectedOption.SetPathCreator(
                    (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
                wasTextureMoved = false;
            }
            PathEditor.ConnectPaths(tempPath, 0);

            string objectPath = "Assets/Prefabs/Enemies/Enemy.prefab";
            EditorGUI.BeginChangeCheck();
            GameObject obj = (GameObject)EditorGUILayout.ObjectField("Prefab", enemyPrefab, typeof(GameObject), false);
            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject(this, "Change prefab");
                enemyPrefab = obj;
                objectPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                isIncorrectPrefab = objectPath.IndexOf("Assets/Prefabs/Enemies/") != 0;
                if (!isIncorrectPrefab)
                {
                    prefabName.stringValue = objectPath.Substring(objectPath.LastIndexOf('/') + 1);
                    prefabName.stringValue = prefabName.stringValue.Substring(0, prefabName.stringValue.LastIndexOf('.') );
                    Debug.Log(prefabName.stringValue);
                    enemySpawnPosition = enemyPrefab.transform.position;
                    enemySprite = enemyPrefab.GetComponent<SpriteRenderer>().sprite;
                    enemyScale = enemyPrefab.transform.localScale;
                }
            }
            if (isIncorrectPrefab)
                EditorGUILayout.HelpBox("The provided object isn't in the enemies folder!", MessageType.Warning);

            EditorGUILayout.PropertyField(enemyName);
            enemySpawnPosition = EditorGUILayout.Vector2Field("Spawn Position", enemySpawnPosition);

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

    public void OnSceneGUI()
    {
        if (foldedOut.intValue == -1 || selectedEnemy == null) return;
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
