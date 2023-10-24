using UnityEngine;
using UnityEditor;
using WaypointPath;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection, tempPath;

    WaypointPathData pathData;
    SerializedProperty path;
    private void OnEnable()
    {
        if (pathData == null) pathData = target as WaypointPathData;
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));
        data.Init();

        int newId = pathData.GetInstanceID();
        if (data.PrefabID != newId)
        {
            PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = data.SelectedPathIndex = pathData.Path.Count - 2;
            if (data.SelectedPathIndex < 0) PathEditor.StartDeleteIndex = PathEditor.EndDeleteIndex = data.SelectedPathIndex = 0;
            data.TempPath.Clear();
            data.PrefabID = newId;
        }
        if (data.TempPath != null && data.TempPath.Count != 0)
        {
            int insert = data.IsInsert ? 2 : 1;
            if (data.TempPath.Count > pathData.Path.Count + insert)
                data.TempPath.RemoveRange(pathData.Path.Count, data.TempPath.Count - (pathData.Path.Count + insert));
        }
        else
        {
            if (pathData.Path == null || pathData.Path.Count == 0)
                data.TempPath = new() { new WaypointPathExpression() };
            else if (pathData.Path != null && pathData.Path.Count != 0)
            {
                foreach (var creator in pathData.Path)
                    data.TempPath.Add(creator.GetNewAdjoinedPath(0));
                data.TempPath.Add(pathData.Path[^1].GetNewAdjoinedPath(1));
            }
        }
        //if (data.SelectedPathIndex < data.TempPath.Count)
        //{
        data.PathTypeSelection = WaypointPathEditorData.GetSelectedOption(data.TempPath[data.SelectedPathIndex]);
        data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);
        //}

        serialData = new SerializedObject(data);
        foreach (var option in WaypointPathEditorData.Options) option.StartPosition = pathData.transform.position;
        pathData.transform.hasChanged = false;

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");

        path = serializedObject.FindProperty("Path");

        Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;
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

        if (tempPath.arraySize > 1 && pathData.Path.Count == 0)
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

        if (pathData.transform.hasChanged)
        {
            if (path.arraySize > 0)
            {
                WaypointPathCreator p = (WaypointPathCreator)path.GetArrayElementAtIndex(0).managedReferenceValue;
                p.StartPosition = pathData.transform.position;
                PathEditor.ConnectPaths(path, 0);
            }
            if (tempPath.arraySize > 0)
            {
                WaypointPathCreator p = (WaypointPathCreator)tempPath.GetArrayElementAtIndex(0).managedReferenceValue;
                p.StartPosition = pathData.transform.position;
                PathEditor.ConnectPaths(tempPath, 0);
            }
            data.SelectedOption.SetPathCreator(
                (WaypointPathCreator)tempPath.GetArrayElementAtIndex(selectedPathIndex.intValue).managedReferenceValue);
            pathData.transform.hasChanged = false;
        }
        PathEditor.ConnectPaths(tempPath, 0);

        data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, pathData.Path);

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
            EditorUtility.SetDirty(pathData);
        }

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        //if (e != EventType.Repaint) return;
        //List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        data.SelectedOption.DrawPath(pathData.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);
        Repaint();
    }
}
