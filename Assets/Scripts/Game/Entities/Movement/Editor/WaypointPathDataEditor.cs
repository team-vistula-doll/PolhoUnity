using UnityEngine;
using UnityEditor;
using WaypointPath;
using System.Linq;

[CustomEditor(typeof(WaypointPathData))]
public class WaypointPathDataEditor : Editor
{
    WaypointPathEditorData data;
    const string assetPath = "Assets/Editor Assets/WaypointPathEditorData.asset";

    SerializedObject serialData;
    SerializedProperty selectedPathIndex, isInsert, pathTypeSelection, tempPath;
    //bool tempIsInsert = false;

    WaypointPathData pathData;
    SerializedObject serialPath;
    SerializedProperty path;

    private void OnEnable()
    {
        if (pathData == null) pathData = target as WaypointPathData;
        serialPath = serializedObject;
        data = (WaypointPathEditorData)AssetDatabase.LoadAssetAtPath(assetPath, typeof(WaypointPathEditorData));
        if (data == null) data = (WaypointPathEditorData)ScriptableObject.CreateInstance(typeof(WaypointPathEditorData));

        data.TempPath = new();
        if (pathData.Path == null || pathData.Path.Count == 0) data.TempPath.Add(new WaypointPathExpression());
        else
        {
            foreach (var creator in pathData.Path)
                data.TempPath.Add(creator.GetNewAdjoinedPath(0));
            data.TempPath.Add(pathData.Path.Last().GetNewAdjoinedPath(1));
        }
        if (data.SelectedPathIndex < data.TempPath.Count)
            data.SelectedOption.SetPathCreator(data.TempPath[data.SelectedPathIndex]);

        serialData = new SerializedObject(data);
        data.SelectedOption.objectTransform = pathData.transform;
        pathData.transform.hasChanged = false;

        selectedPathIndex = serialData.FindProperty("SelectedPathIndex");
        isInsert = serialData.FindProperty("IsInsert");
        pathTypeSelection = serialData.FindProperty("PathTypeSelection");
        tempPath = serialData.FindProperty("TempPath");

        path = serialPath.FindProperty("Path");

        Undo.undoRedoPerformed -= UndoRedo; Undo.undoRedoPerformed += UndoRedo;
    }

    private void UndoRedo()
    {
        //Debug.Log("UndoRedo call in WaypointPathDataEditor");
        data.SelectedOption.ConnectPaths(data.TempPath, 0);
        if (selectedPathIndex.intValue < data.TempPath.Count
            && data.SelectedOption.GetPathCreator() != data.TempPath[selectedPathIndex.intValue])
            data.SelectedOption.SetPathCreator(data.TempPath[selectedPathIndex.intValue]);
        else data.SelectedOption.ApplyPathOptions();
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
        //var pathCreator = data.SelectedOption.GetPathCreator();

        if (tempPath.arraySize > 1 && pathData.Path.Count == 0)
        {
            tempPath.ClearArray();
            if (pathData.Path == null || pathData.Path.Count == 0)
            {
                tempPath.arraySize++;
                tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = new WaypointPathExpression();
            }
            else
            {
                foreach (var path in pathData.Path)
                {
                    tempPath.arraySize++;
                    tempPath.GetArrayElementAtIndex(tempPath.arraySize - 1).managedReferenceValue = path.GetNewAdjoinedPath(0);
                }
                data.TempPath.Add(pathData.Path.Last().GetNewAdjoinedPath(0));
                data.SelectedOption.ConnectPaths(tempPath, 0);
            }
            data.SelectedOption.SetPathCreator(data.TempPath[^1]);
        }

        if (pathData.transform.hasChanged)
        {
            data.SelectedOption.objectTransform = pathData.transform;
            data.SelectedOption.ConnectPaths(pathData.Path, 0);
            serializedObject.UpdateIfRequiredOrScript();
            data.SelectedOption.ConnectPaths(tempPath, 0);
            pathData.transform.hasChanged = false;
        }

        data.SelectedOption.SelectPath(selectedPathIndex, pathTypeSelection, isInsert, tempPath, pathData.Path);
        //{
        //    if (selectedPathIndex.intValue < pathData.Path.Count)
        //        tempPath[selectedPathIndex.intValue] = pathCreator.GetNewAdjoinedPath(0);
        //    data.SelectedOption.startDeleteIndex = data.SelectedOption.endDeleteIndex = selectedPathIndex.intValue;
        //}

        selectedPathIndex.intValue -= data.SelectedOption.DeletePath(path, tempPath);
        if (selectedPathIndex.intValue < 0) selectedPathIndex.intValue = 0;

        EditorGUILayout.Space();
        PathEditor.PathTypes(pathTypeSelection);

        if (data.SelectedOption.PathOptions()) data.SelectedOption.ConnectPaths(data.TempPath, selectedPathIndex.intValue);

        //if (!isInsert.boolValue && tempIsInsert && tempPath.Count > 0 && selectedPathIndex.intValue < tempPath.Count)
        //{
        //    tempPath.RemoveAt(selectedPathIndex.intValue);
        //    tempIsInsert = false;
        //}
        //if ((isInsert.boolValue && !tempIsInsert) || tempPath.Count == 0 || selectedPathIndex.intValue >= tempPath.Count)
        //{
        //    tempPath.Insert(selectedPathIndex.intValue, pathCreator.GetNewAdjoinedPath(0));
        //    if ((isInsert.boolValue && !tempIsInsert)) tempIsInsert = true;
        //}
        //if (pathOptionsChanged) data.SelectedOption.ConnectPaths(tempPath, selectedPathIndex.intValue);

        data.SelectedOption.SetPath(path, isInsert, selectedPathIndex, tempPath);

        EditorGUILayout.Space();
        DrawPropertiesExcluding(serializedObject, "m_Script");

        serialData.ApplyModifiedProperties();
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    public void OnSceneGUI()
    {
        EventType e = Event.current.type;
        if (e != EventType.Repaint) return;
        //List<Vector2> vector2s = PathEditor.CreateVectorPath(in pathData.Path, 0);
        data.SelectedOption.DrawPath(pathData.Path, 0, e, false);
        data.SelectedOption.DrawPath(data.TempPath, selectedPathIndex.intValue, e, true);
    }
}
